//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright (c) 2019 Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

namespace DotLang.CodeAnalysis.Syntax
{
    using static SyntaxKind;

    /// <summary>
    /// Various helper methods for working with <see cref="SyntaxKind"/> and
    /// <see cref="SyntaxToken"/>.
    /// </summary>
    public static class SyntaxFacts
    {
        /// <summary>
        /// Determines if a <see cref="SyntaxKind"/> is valid. That is,
        /// any kind except <see cref="None"/>.
        /// </summary>
        public static bool IsValid(this SyntaxKind kind)
            => kind != None;

        /// <summary>
        /// Determines if a <see cref="SyntaxToken"/> is valid. That is,
        /// a token whose <see cref="SyntaxToken.Kind"/> is any kind except
        /// <see cref="None"/>.
        /// </summary>
        public static bool IsValid(this SyntaxToken token)
            => token.Kind.IsValid();

        /// <summary>
        /// Determines if a <see cref="SyntaxKind"/> is an identifier.
        /// That is, any kind that is one of <see cref="IdentifierToken"/>,
        /// <see cref="StringLiteralToken"/>, <see cref="XmlLiteralToken"/>,
        /// or <see cref="NumeralLiteralToken"/>.
        /// </summary>
        public static bool IsIdentifier(this SyntaxKind kind)
            => kind == IdentifierToken
                || kind == StringLiteralToken
                || kind == XmlLiteralToken
                || kind == NumeralLiteralToken;

        /// <summary>
        /// Determines if a <see cref="SyntaxToken"/> is an identifier.
        /// That is a token whose <see cref="SyntaxToken.Kind"/> is one of
        /// <see cref="IdentifierToken"/>, <see cref="StringLiteralToken"/>,
        /// <see cref="XmlLiteralToken"/>, or <see cref="NumeralLiteralToken"/>.
        /// </summary>
        public static bool IsIdentifier(this SyntaxToken token)
            => token.Kind.IsIdentifier();

        /// <summary>
        /// Determines if a <see cref="SyntaxKind"/> is an edge.
        /// That is, any kind that is one of <see cref="UndirectedEdgeToken"/>,
        /// or <see cref="DirectedEdgeToken"/>.
        /// </summary>
        public static bool IsEdge(this SyntaxKind kind)
            => kind == DirectedEdgeToken
                || kind == UndirectedEdgeToken;

        /// <summary>
        /// Determines if a <see cref="SyntaxToken"/> is an edge.
        /// That is a token whose <see cref="SyntaxToken.Kind"/> is one of
        /// <see cref="UndirectedEdgeToken"/> or <see cref="DirectedEdgeToken"/>.
        /// </summary>
        public static bool IsEdge(this SyntaxToken token)
            => token.Kind.IsEdge();

        /// <summary>
        /// Determines if a <see cref="SyntaxKind"/> is trivia.
        /// That is, any kind that is one of <see cref="WhiteSpaceTriviaToken"/>,
        /// <see cref="SingleLineCommentTriviaToken"/>,
        /// or <see cref="MultiLineCommentTriviaToken"/>.
        /// </summary>
        public static bool IsTrivia(this SyntaxKind kind)
            => kind == WhiteSpaceTriviaToken
                || kind == SingleLineCommentTriviaToken
                || kind == MultiLineCommentTriviaToken;

        /// <summary>
        /// Determines if a <see cref="SyntaxToken"/> is trivia.
        /// That is a token whose <see cref="SyntaxToken.Kind"/> is one of
        /// <see cref="WhiteSpaceTriviaToken"/>,
        /// <see cref="SingleLineCommentTriviaToken"/>,
        /// or <see cref="MultiLineCommentTriviaToken"/>.
        /// </summary>
        public static bool IsTrivia(this SyntaxToken token)
            => token.Kind.IsTrivia();

        /// <summary>
        /// Determines if a character is a valid identifier character for
        /// an <see cref="IdentifierToken"/>.
        /// </summary>
        /// <param name="c">The character to validate.</param>
        /// <param name="firstChar">
        /// Whether or not <paramref name="c"/> is the first character
        /// in the potential <see cref="IdentifierToken"/>.
        /// </param>
        public static bool IsIdentifierChar(char c, bool firstChar)
        {
            if (c == '_')
            {
                return true;
            }

            // Unicode Character 'REPLACEMENT CHARACTER' (U+FFFD)
            // will be seen for characters that cannot be converted.
            if (c == '\ufffd')
            {
                return true;
            }

            return firstChar
                ? char.IsLetter(c)
                : char.IsLetterOrDigit(c);
        }

        /// <summary>
        /// Returns a text representation of a <see cref="SyntaxKind"/> for kinds that
        /// have a static representation (character literal tokens and keywords), or
        /// an empty string for kinds that cannot be statically represented.
        /// </summary>
        public static string GetText(this SyntaxKind kind)
        {
            switch (kind)
            {
                case EqualsToken:
                    return "=";
                case OpenBraceToken:
                    return "{";
                case CloseBraceToken:
                    return "}";
                case OpenBracketToken:
                    return "[";
                case CloseBracketToken:
                    return "]";
                case ColonToken:
                    return ":";
                case SemicolonToken:
                    return ";";
                case CommaToken:
                    return ",";
                case DirectedEdgeToken:
                    return "->";
                case UndirectedEdgeToken:
                    return "--";
                case StrictKeyword:
                    return "strict";
                case GraphKeyword:
                    return "graph";
                case DigraphKeyword:
                    return "digraph";
                case SubgraphKeyword:
                    return "subgraph";
                case NodeKeyword:
                    return "node";
                case EdgeKeyword:
                    return "edge";
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Determines if <paramref name="c"/> is a suitable terminal delimeter
        /// such that no explicit <c>;</c> needs to be inserte dbetween tokens.
        /// </summary>
        public static bool IsTerminalDelimiterChar(char c)
        {
            switch (c)
            {
                case ':':
                case ';':
                case '[':
                case ']':
                case '}':
                    return true;
                default:
                    return char.IsWhiteSpace(c);
            }
        }

        /// <summary>
        /// Returns a <see cref="SyntaxKind"/> for text that can be parsed or <see cref="None"/>
        /// if <paramref name="text"/> cannot be parsed.
        /// </summary>
        public static SyntaxKind GetKind(string text)
        {
            switch (text)
            {
                case null:
                case "":
                    return None;
                case "=":
                    return EqualsToken;
                case "{":
                    return OpenBraceToken;
                case "}":
                    return CloseBraceToken;
                case "[":
                    return OpenBracketToken;
                case "]":
                    return CloseBracketToken;
                case ":":
                    return ColonToken;
                case ";":
                    return SemicolonToken;
                case ",":
                    return CommaToken;
                case "->":
                    return DirectedEdgeToken;
                case "--":
                    return UndirectedEdgeToken;
                default:
                    switch (text.ToLowerInvariant())
                    {
                        case "strict":
                            return StrictKeyword;
                        case "graph":
                            return GraphKeyword;
                        case "digraph":
                            return DigraphKeyword;
                        case "subgraph":
                            return SubgraphKeyword;
                        case "node":
                            return NodeKeyword;
                        case "edge":
                            return EdgeKeyword;
                    }

                    return None;
            }
        }

        public static SyntaxToken ToIdentifierToken(string? id)
        {
            if (id is null)
            {
                return SyntaxToken.Invalid;
            }

            if (id == string.Empty || id == ".")
            {
                return new SyntaxToken(StringLiteralToken, id);
            }

            var numberChars = 0;
            var dotChars = 0;
            var firstCharIsDigit = false;

            for (int i = 0; i < id.Length; i++)
            {
                var c = id[i];

                if (char.IsNumber(c))
                {
                    numberChars++;
                    if (i == 0)
                    {
                        firstCharIsDigit = true;
                    }
                }
                else if (!IsIdentifierChar(c, firstChar: i == 0))
                {
                    if (c == '.')
                    {
                        dotChars++;
                    }

                    if (c != '.' || dotChars > 1)
                    {
                        return new SyntaxToken(StringLiteralToken, id);
                    }
                }
            }

            if (dotChars < 2 && numberChars + dotChars == id.Length)
            {
                return new SyntaxToken(NumeralLiteralToken, id);
            }

            if (firstCharIsDigit || dotChars > 0)
            {
                return new SyntaxToken(StringLiteralToken, id);
            }

            return new SyntaxToken(IdentifierToken, id);
        }
    }
}
