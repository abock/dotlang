//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright (c) 2019 Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;

using Xunit;

using DotLang.CodeAnalysis.Syntax;

namespace DotLang.Tests
{
    using static SyntaxKind;

    public class LexerTests
    {
        [Theory]
        [InlineData("{", OpenBraceToken)]
        [InlineData("{}", OpenBraceToken, CloseBraceToken)]
        [InlineData("{}[]", OpenBraceToken, CloseBraceToken, OpenBracketToken, CloseBracketToken)]
        [InlineData("=:;,", EqualsToken, ColonToken, SemicolonToken, CommaToken)]
        public void SingleChars(string sourceText, params SyntaxKind[] expectedTokens)
        {
            var lexer = new Lexer(sourceText);
            int i = 0;

            while (true)
            {
                var token = lexer.ReadToken();
                if (token.Kind == EndOfFileToken)
                    break;

                Assert.Equal(expectedTokens[i], token.Kind);
                Assert.Equal(
                    sourceText[i].ToString(),
                    token.SourceText ?? token.Kind.GetText());
                Assert.Equal(i, token.Location.SourceSpan.Start);
                Assert.Equal(1, token.Location.SourceSpan.Length);

                i++;
            }
        }

        [Theory]
        [InlineData("a", IdentifierToken, "a")]
        [InlineData("aa", IdentifierToken, "aa")]
        [InlineData("a aa a0 _0A", IdentifierToken, "a", "aa", "a0", "_0A")]
        [InlineData("\"\"", StringLiteralToken, "")]
        [InlineData("\"\\\"\"", StringLiteralToken, "\"")]
        [InlineData("\"\\\"\\\"\"", StringLiteralToken, "\"\"")]
        [InlineData("\"hello world\"", StringLiteralToken, "hello world")]
        [InlineData("\"hello \\\"world\\\"\"", StringLiteralToken, "hello \"world\"")]
        [InlineData("\"random\\r\\te\\l\\nscapes\"", StringLiteralToken, "random\\r\\te\\l\\nscapes")]
        [InlineData("0", NumeralLiteralToken, "0")]
        [InlineData("12", NumeralLiteralToken, "12")]
        [InlineData("-0", NumeralLiteralToken, "-0")]
        [InlineData("-.5", NumeralLiteralToken, "-.5")]
        [InlineData(".5", NumeralLiteralToken, ".5")]
        [InlineData("0.5", NumeralLiteralToken, "0.5")]
        [InlineData("-0.5", NumeralLiteralToken, "-0.5")]
        [InlineData("-000.500", NumeralLiteralToken, "-000.500")]
        [InlineData("1.2.3.4", NumeralLiteralToken, "1.2", ".3", ".4")]
        [InlineData("1.2.3-.4", NumeralLiteralToken, "1.2", ".3", "-.4")]
        [InlineData("<>", XmlLiteralToken, "")]
        [InlineData("<><><>", XmlLiteralToken, "", "", "")]
        [InlineData("<a><><b><><c>", XmlLiteralToken, "a", "", "b", "", "c")]
        [InlineData("<<a/>><><<b/>><><<c/>>", XmlLiteralToken, "<a/>", "", "<b/>", "", "<c/>")]
        [InlineData("<<![CDATA[woo]]>>", XmlLiteralToken, "<![CDATA[woo]]>")]
        [InlineData(
            @"<<?xml version=""1.0"" encoding=""utf-8""?>
            <configuration>
              <packageSources>
                <add
                  key=""nuget.org""
                  value=""https://api.nuget.org/v3/index.json""
                  protocolVersion=""3""/>
              </packageSources>
            </configuration>>",
            XmlLiteralToken,
            @"<?xml version=""1.0"" encoding=""utf-8""?>
            <configuration>
              <packageSources>
                <add
                  key=""nuget.org""
                  value=""https://api.nuget.org/v3/index.json""
                  protocolVersion=""3""/>
              </packageSources>
            </configuration>")]
        public void Identifiers(
            string sourceText,
            SyntaxKind expectedKind,
            params string[] expectedLiterals)
        {
            var lexer = new Lexer(sourceText);
            int i = 0;

            while (true)
            {
                var token = lexer.ReadToken();
                if (token.Kind == EndOfFileToken)
                    break;

                Assert.Equal(expectedKind, token.Kind);
                Assert.Equal(
                    expectedLiterals[i].ToString(),
                    token.StringValue);

                i++;
            }
        }

        [Theory]
        [InlineData("strict", StrictKeyword)]
        [InlineData("graph", GraphKeyword)]
        [InlineData("digraph", DigraphKeyword)]
        [InlineData("subgraph", SubgraphKeyword)]
        [InlineData("node", NodeKeyword)]
        [InlineData("edge", EdgeKeyword)]
        // not actually keywords (compass)
        [InlineData("n", IdentifierToken)]
        [InlineData("ne", IdentifierToken)]
        [InlineData("e", IdentifierToken)]
        [InlineData("se", IdentifierToken)]
        [InlineData("s", IdentifierToken)]
        [InlineData("sw", IdentifierToken)]
        [InlineData("w", IdentifierToken)]
        [InlineData("nw", IdentifierToken)]
        [InlineData("c", IdentifierToken)]
        [InlineData("_", IdentifierToken)]
        public void Keywords(string sourceText, SyntaxKind expectedKind)
        {
            var lexer = new Lexer(sourceText);
            var token = lexer.ReadToken();

            Assert.Equal(expectedKind, token.Kind);
            Assert.Equal(sourceText, token.StringValue);
        }

        [Theory]
        [InlineData("//", "//")]
        [InlineData("//\n", "//", "\n")]
        [InlineData("    //    \n", "    ", "//    ", "\n")]
        [InlineData("    \n\n\n\n    ", "    \n\n\n\n    ")]
        [InlineData("////", "////")]
        [InlineData("//\n//", "//", "\n", "//")]
        [InlineData("/* hello */", "/* hello */")]
        [InlineData("/** *//***//****/", "/** */", "/***/", "/****/")]
        [InlineData("/**\n**/\n//*\n", "/**\n**/", "\n", "//*", "\n")]
        public void Trivia(string sourceText, params string[] expectedTrivia)
        {
            var lexer = new Lexer(sourceText);
            var token = lexer.ReadToken();

            Assert.Equal(EndOfFileToken, token.Kind);

            var inspectors = new Action<SyntaxToken>[expectedTrivia.Length];
            for (int i = 0; i < inspectors.Length; i++)
            {
                var expected = expectedTrivia[i];
                inspectors[i] = actual =>
                {
                    SyntaxKind expectedKind = None;

                    if (expected.StartsWith("//", StringComparison.Ordinal))
                    {
                        expectedKind = SingleLineCommentTriviaToken;
                    }
                    else if (expected.StartsWith("/*", StringComparison.Ordinal))
                    {
                        expectedKind = MultiLineCommentTriviaToken;
                    }
                    else
                    {
                        expectedKind = WhiteSpaceTriviaToken;
                    }

                    Assert.Equal(expected, actual.SourceText);
                    Assert.Equal(expectedKind, actual.Kind);
                };
            }

            Assert.Collection(token.LeadingTrivia, inspectors);
        }

        [Theory]
        [InlineData("#", "#")]
        [InlineData("#\n", "#", "\n")]
        [InlineData(" #", " ", "#")]
        [InlineData("  \t#", "  \t", "#")]
        [InlineData("   \t  #", "   \t  ", "#")]
        [InlineData(" #\n", " ", "#", "\n")]
        [InlineData("  \t#\n", "  \t", "#", "\n")]
        [InlineData("   \t  #\n", "   \t  ", "#", "\n")]
        [InlineData("   \t  #line 34\n", "   \t  ", "#line 34", "\n")]
        [InlineData("#if 0", "#if 0")]
        public void PreprocessorDirectives(string sourceText, params string[] expectedTokens)
        {
            var lexer = new Lexer(sourceText);

            int i = 0;
            foreach (var token in lexer.ReadTokens(promoteTrivia: true))
            {
                // This is a test for trivia promotion - trivia should be
                // detached from the original primary token.
                Assert.Empty(token.LeadingTrivia);

                if (token.Kind == EndOfFileToken)
                {
                    break;
                }

                var expected = expectedTokens[i++];

                Assert.Equal(
                    string.IsNullOrWhiteSpace(expected)
                        ? SyntaxKind.WhiteSpaceTriviaToken
                        : SyntaxKind.PreprocessorDirective,
                    token.Kind);

                Assert.Equal(expected, token.SourceText);
            }
        }

        [Fact]
        public void InvalidPreprocessorDirective()
        {
            new Lexer("#line 34\ngraph { }")
                .ReadTokens()
                .ToList();

            Assert.ThrowsAny<Exception>(() =>
            {
                new Lexer("graph { } #line 34")
                    .ReadTokens()
                    .ToList();
            });
        }

        [Fact]
        public void PeekToken()
        {
            var lexer = new Lexer("graph { a -> b c -> d }");

            Assert.Equal(GraphKeyword, lexer.PeekToken().Kind);
            Assert.Equal(GraphKeyword, lexer.PeekToken().Kind);
            Assert.Equal(GraphKeyword, lexer.ReadToken().Kind);

            Assert.Equal(OpenBraceToken, lexer.PeekToken().Kind);
            Assert.Equal(OpenBraceToken, lexer.PeekToken().Kind);
            Assert.Equal(OpenBraceToken, lexer.ReadToken().Kind);

            Assert.Equal(IdentifierToken, lexer.PeekToken().Kind);
            Assert.Equal(IdentifierToken, lexer.PeekToken().Kind);
            Assert.Equal(IdentifierToken, lexer.ReadToken().Kind);

            Assert.Equal(DirectedEdgeToken, lexer.PeekToken().Kind);
            Assert.Equal(DirectedEdgeToken, lexer.PeekToken().Kind);
            Assert.Equal(DirectedEdgeToken, lexer.ReadToken().Kind);

            Assert.Equal(IdentifierToken, lexer.PeekToken().Kind);
            Assert.Equal(IdentifierToken, lexer.PeekToken().Kind);
            Assert.Equal(IdentifierToken, lexer.ReadToken().Kind);

            Assert.Equal(IdentifierToken, lexer.PeekToken().Kind);
            Assert.Equal(IdentifierToken, lexer.PeekToken().Kind);
            Assert.Equal(IdentifierToken, lexer.ReadToken().Kind);

            Assert.Equal(DirectedEdgeToken, lexer.PeekToken().Kind);
            Assert.Equal(DirectedEdgeToken, lexer.PeekToken().Kind);
            Assert.Equal(DirectedEdgeToken, lexer.ReadToken().Kind);

            Assert.Equal(IdentifierToken, lexer.PeekToken().Kind);
            Assert.Equal(IdentifierToken, lexer.PeekToken().Kind);
            Assert.Equal(IdentifierToken, lexer.ReadToken().Kind);

            Assert.Equal(CloseBraceToken, lexer.PeekToken().Kind);
            Assert.Equal(CloseBraceToken, lexer.PeekToken().Kind);
            Assert.Equal(CloseBraceToken, lexer.ReadToken().Kind);

            Assert.Equal(EndOfFileToken, lexer.PeekToken().Kind);
            Assert.Equal(EndOfFileToken, lexer.PeekToken().Kind);
            Assert.Equal(EndOfFileToken, lexer.ReadToken().Kind);
        }
    }
}
