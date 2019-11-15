//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright (c) 2019 Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;

using DotLang.CodeAnalysis.Text;

namespace DotLang.CodeAnalysis.Syntax
{
    using static SyntaxKind;

    /// <summary>
    /// Separates arbitrary source text into a stream of <see cref="SyntaxToken"/>.
    /// </summary>
    public sealed class Lexer
    {
        struct TokenInfo
        {
            public SyntaxKind Kind;
            public int SpanStart;
            public int Line;
            public int Column;
            public string? StringValue;
        }

        readonly SourceText sourceText;
        readonly TextWindow textWindow;
        readonly StringBuilder stringBuilder = new StringBuilder();

        public Lexer(string sourceText)
            : this(SourceText.From(sourceText))
        {
        }

        public Lexer(SourceText sourceText)
        {
            this.sourceText = sourceText
                ?? throw new ArgumentNullException(nameof(sourceText));

            this.textWindow = new TextWindow(sourceText);
        }

        SyntaxToken currentToken = SyntaxToken.Invalid;
        SyntaxToken nextToken = SyntaxToken.Invalid;

        /// <summary>
        /// Consumes the next token in the source text.
        /// </summary>
        public SyntaxToken ReadToken()
        {
            if (nextToken.Kind == None)
            {
                currentToken = Lex();
                if (currentToken.Kind != EndOfFileToken)
                {
                    nextToken = Lex();
                }
            }
            else
            {
                currentToken = nextToken;
                nextToken = Lex();
            }

            return currentToken;
        }

        /// <summary>
        /// Peeks at the next token in the source text but does not consume
        /// it. Invoke <see cref="ReadToken"/> to consume the token.
        /// </summary>
        public SyntaxToken PeekToken()
        {
            if (currentToken.Kind == None)
            {
                currentToken = Lex();
                nextToken = currentToken;
            }

            return nextToken;
        }

        /// <summary>
        /// Reads all tokens in the source text, yielding as they are ready,
        /// until <see cref="EndOfFileToken"/> is seen.
        /// </summary>
        /// <param name="promoteTrivia">
        /// If <c>true</c>, leading trivia is detached from non-trivia tokens
        /// and yielded in order as if the triva was not trivia. For example:
        /// ```text
        /// ID  →  `=`  →  ID  →  EOF
        /// ↑       ↑      ↑
        /// SPACE   SPACE  SPACE
        /// ```
        /// Becomes:
        /// ```text
        /// SPACE → ID → SPACE → `=` → SPACE → ID → EOF
        /// ```
        /// </param>
        public IEnumerable<SyntaxToken> ReadTokens(bool promoteTrivia = false)
        {
            while (true)
            {
                var token = ReadToken();

                if (promoteTrivia)
                {
                    foreach (var trivia in token.LeadingTrivia)
                    {
                        yield return trivia;
                    }

                    yield return new SyntaxToken(
                        token.Kind,
                        token.Location,
                        token.SourceText,
                        token.StringValue,
                        Array.Empty<SyntaxToken>());
                }
                else
                {
                    yield return token;
                }

                if (token.Kind == EndOfFileToken)
                {
                    break;
                }
            }
        }

        SyntaxToken Create(
            in TokenInfo tokenInfo,
            List<SyntaxToken>? leadingTrivia = null)
            => new SyntaxToken(
                kind: tokenInfo.Kind,
                location: new SourceLocation(
                    TextSpan.FromBounds(tokenInfo.SpanStart, textWindow.Position),
                    new LinePositionSpan(
                        new LinePosition(tokenInfo.Line, tokenInfo.Column),
                        new LinePosition(textWindow.Line, textWindow.Position))),
                sourceText: sourceText.ToString(
                    TextSpan.FromBounds(
                        tokenInfo.SpanStart,
                        textWindow.Position)),
                stringValue: tokenInfo.StringValue,
                leadingTrivia: ListHelper.ListOrEmpty(leadingTrivia));

        SyntaxToken Lex()
        {
            List<SyntaxToken>? leadingTrivia = null;

            while (true)
            {
                var triviaInfo = Lex(triviaMode: true);
                if (triviaInfo.Kind == None)
                {
                    break;
                }

                ListHelper.Append(
                    ref leadingTrivia,
                    Create(in triviaInfo));
            }

            var tokenInfo = Lex(triviaMode: false);
            return Create(in tokenInfo, leadingTrivia);
        }

        TokenInfo Lex(bool triviaMode)
        {
            var info = new TokenInfo();
            info.Kind = None;
            info.StringValue = null;
            info.SpanStart = textWindow.Position;
            info.Line = textWindow.Line;
            info.Column = textWindow.Column;

            char c = textWindow.PeekChar();

            if (triviaMode)
            {
                if (char.IsWhiteSpace(c))
                {
                    while (char.IsWhiteSpace(textWindow.PeekChar()))
                    {
                        if (textWindow.NextChar() == TextWindow.InvalidCharacter)
                        {
                            break;
                        }
                    }

                    info.Kind = WhiteSpaceTriviaToken;
                }
                else if (c == '/')
                {
                    textWindow.NextChar();
                    c = textWindow.PeekChar ();

                    if (c== '/')
                    {
                        while (textWindow.PeekChar () != '\n')
                        {
                            if (textWindow.NextChar() == TextWindow.InvalidCharacter)
                            {
                                break;
                            }
                        }

                        info.Kind = SingleLineCommentTriviaToken;
                    }
                    else if (c == '*')
                    {
                        textWindow.NextChar();
                        info.Kind = MultiLineCommentTriviaToken;

                        while ((c = textWindow.NextChar()) != TextWindow.InvalidCharacter)
                        {
                            if (c == '*' && textWindow.PeekChar() == '/')
                            {
                                textWindow.NextChar();
                                break;
                            }
                        }
                    }
                }

                return info;
            }

            switch (c)
            {
                case TextWindow.InvalidCharacter:
                    info.Kind = EndOfFileToken;
                    break;
                case '{':
                    textWindow.NextChar();
                    info.Kind = OpenBraceToken;
                    break;
                case '}':
                    textWindow.NextChar();
                    info.Kind = CloseBraceToken;
                    break;
                case '[':
                    textWindow.NextChar();
                    info.Kind = OpenBracketToken;
                    break;
                case ']':
                    textWindow.NextChar();
                    info.Kind = CloseBracketToken;
                    break;
                case ',':
                    textWindow.NextChar();
                    info.Kind = CommaToken;
                    break;
                case ';':
                    textWindow.NextChar();
                    info.Kind = SemicolonToken;
                    break;
                case ':':
                    textWindow.NextChar();
                    info.Kind = ColonToken;
                    break;
                case '=':
                    textWindow.NextChar();
                    info.Kind = EqualsToken;
                    break;
                case '#' when textWindow.LineIsWhiteSpaceOnlySoFar:
                    textWindow.NextChar();

                    while (textWindow.PeekChar () != '\n')
                    {
                        if (textWindow.NextChar() == TextWindow.InvalidCharacter)
                        {
                            break;
                        }
                    }

                    info.Kind = PreprocessorDirective;
                    break;
                case '-':
                    textWindow.NextChar();

                    c = textWindow.PeekChar();
                    if (c == '-')
                    {
                        textWindow.NextChar();
                        info.Kind = UndirectedEdgeToken;
                    }
                    else if (c == '>')
                    {
                        textWindow.NextChar();
                        info.Kind = DirectedEdgeToken;
                    }
                    else if (c == '.' || char.IsDigit(c))
                    {
                        LexNumeralLiteral(ref info, c, negate: true);
                    }
                    break;
                case '"':
                    textWindow.NextChar();
                    LexStringLiteral(ref info);
                    break;
                case '<':
                    textWindow.NextChar();
                    LexXmlLiteral(ref info);
                    break;
                default:
                    if (c == '.' || char.IsDigit(c))
                    {
                        LexNumeralLiteral(ref info, c, negate: false);
                        break;
                    }
                    else if (SyntaxFacts.IsIdentifierChar(c, firstChar: true))
                    {
                        LexIdentifier(ref info, c);
                        break;
                    }

                    throw new NotImplementedException($"c={c} ({(int)c})");
            }

            return info;
        }

        void LexNumeralLiteral(ref TokenInfo info, char c, bool negate)
        {
            stringBuilder.Clear();

            if (negate)
            {
                stringBuilder.Append('-');
            }

            stringBuilder.Append(c);
            textWindow.NextChar();

            var seenDot = c == '.';

            while (true)
            {
                c = textWindow.PeekChar();
                if (!seenDot && c == '.')
                {
                    stringBuilder.Append(c);
                    textWindow.NextChar();
                    seenDot = true;
                }
                else if (char.IsDigit(c))
                {
                    stringBuilder.Append(c);
                    textWindow.NextChar();
                }
                else
                {
                    break;
                }
            }

            info.Kind = NumeralLiteralToken;
            info.StringValue = stringBuilder.ToString();
        }

        void LexStringLiteral(ref TokenInfo info)
        {
            stringBuilder.Clear();

            while (true)
            {
                var c = textWindow.NextChar();

                // In quoted strings in DOT, the only escaped character is double-quote (").
                // That is, in quoted strings, the dyad \" is converted to "; all other
                // characters are left unchanged. In particular, \\ remains \\. Layout engines
                // may apply additional escape sequences.
                if (c == '\\')
                {
                    c = textWindow.NextChar();
                    if (c != '"')
                    {
                        stringBuilder.Append('\\');
                    }

                    stringBuilder.Append(c);
                }
                else if (c == TextWindow.InvalidCharacter || c == '"')
                {
                    break;
                }
                else
                {
                    stringBuilder.Append(c);
                }
            }

            info.Kind = StringLiteralToken;
            info.StringValue = stringBuilder.ToString();
        }

        void LexXmlLiteral(ref TokenInfo info)
        {
            stringBuilder.Clear();

            var depth = 0;

            while (true)
            {
                var c = textWindow.NextChar();

                if (c == TextWindow.InvalidCharacter)
                {
                    break;
                }
                else if (c == '<')
                {
                    depth++;
                }
                else if (c == '>')
                {
                    depth--;
                    if (depth < 0)
                    {
                        break;
                    }
                }

                stringBuilder.Append(c);
            }

            info.Kind = XmlLiteralToken;
            info.StringValue = stringBuilder.ToString();
        }

        void LexIdentifier(ref TokenInfo info, char c)
        {
            stringBuilder.Clear();
            stringBuilder.Append(c);
            textWindow.NextChar();

            while (SyntaxFacts.IsIdentifierChar(
                c = textWindow.PeekChar(),
                firstChar: false))
            {
                textWindow.NextChar();
                stringBuilder.Append(c);
            }

            info.StringValue = stringBuilder.ToString();

            info.Kind = SyntaxFacts.GetKind(info.StringValue);
            if (info.Kind == None)
                info.Kind = IdentifierToken;
        }

        sealed class TextWindow
        {
            public const char InvalidCharacter = char.MaxValue;

            readonly SourceText text;

            public int Position { get; private set; }
            public int Line { get; private set; }
            public int Column { get; private set; }
            public bool LineIsWhiteSpaceOnlySoFar { get; private set; } = true;

            public TextWindow(SourceText text)
                => this.text = text;

            public char PeekChar()
                => Position >= text.Length
                    ? InvalidCharacter
                    : text[Position];

            public char NextChar()
            {
                var character = PeekChar();

                if (character != InvalidCharacter)
                {
                    Position++;

                    if (!char.IsWhiteSpace(character))
                    {
                        LineIsWhiteSpaceOnlySoFar = false;
                    }

                    if (character == '\n')
                    {
                        Line++;
                        Column = 0;
                        LineIsWhiteSpaceOnlySoFar = true;
                    }
                    else
                    {
                        Column++;
                    }
                }

                return character;
            }
        }
    }
}
