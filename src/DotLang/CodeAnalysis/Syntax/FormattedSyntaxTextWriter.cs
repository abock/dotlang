//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright (c) 2019 Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;

namespace DotLang.CodeAnalysis.Syntax
{
    using static SyntaxKind;

    sealed class FormattedSyntaxTextWriter
    {
        readonly TextWriter baseTextWriter;
        readonly FormattedSyntaxTextWriterSettings settings;

        int braceDepth;
        char lastCharWritten;

        public FormattedSyntaxTextWriter(
            TextWriter baseTextWriter,
            FormattedSyntaxTextWriterSettings settings)
        {
            this.baseTextWriter = baseTextWriter
                ?? throw new ArgumentNullException(nameof(baseTextWriter));

            this.settings = settings
                ?? throw new ArgumentNullException(nameof(settings));
        }

        public void Write(char c)
        {
            if (lastCharWritten == settings.LastNewLineChar)
            {
                for (int i = 0; i < braceDepth; i++)
                {
                    baseTextWriter.Write(settings.Indent);
                }
            }

            lastCharWritten = c;
            baseTextWriter.Write(c);
        }

        public void Write(string? str)
        {
            if (str is string)
            {
                for (int i = 0; i < str.Length; i++)
                {
                    Write(str[i]);
                }
            }
        }

        public void WriteLine()
            => Write(settings.NewLine);

        public void WriteOpenBrace()
        {
            braceDepth++;
            WriteToken(new SyntaxToken(OpenBraceToken));
            WriteLine();
        }

        public void WriteCloseBrace()
        {
            braceDepth--;
            WriteToken(new SyntaxToken(CloseBraceToken));
        }

        public void WriteDelimiterIfNeeded(char delimiter = ' ')
        {
            if (lastCharWritten == ']' && delimiter == ';')
            {
                Write(delimiter);
            }
            else if (lastCharWritten != char.MinValue
                && lastCharWritten != delimiter
                && !SyntaxFacts.IsTerminalDelimiterChar(lastCharWritten))
            {
                Write(delimiter);
            }
        }

        public void WriteToken(SyntaxKind kind)
            => WriteToken(new SyntaxToken(kind));

        public void WriteToken(SyntaxToken token, string? text = null)
        {
            if (!token.IsValid())
            {
                return;
            }

            if (text is string)
            {
                WriteDelimiterIfNeeded();
                Write(text);
            }
            else if (!SyntaxFacts.IsIdentifier(token))
            {
                switch(token.Kind)
                {
                    case OpenBracketToken:
                    case CloseBracketToken:
                    case ColonToken:
                        break;
                    default:
                        WriteDelimiterIfNeeded();
                        break;
                }

                Write(SyntaxFacts.GetText(token.Kind));
            }
            else if (token.StringValue is null)
            {
                return;
            }
            else
            {
                WriteDelimiterIfNeeded();
                switch (token.Kind)
                {
                    case StringLiteralToken:
                        Write('"');

                        for (int i = 0; i < token.StringValue.Length; i++)
                        {
                            var c = token.StringValue[i];

                            if (c == '"')
                            {
                                Write('\\');
                            }

                            Write(c);
                        }

                        Write('"');
                        break;
                    case XmlLiteralToken:
                        Write('<');
                        Write(token.StringValue);
                        Write('>');
                        break;
                    default:
                        Write(token.StringValue);
                        break;
                }
            }
        }
    }
}
