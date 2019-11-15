//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright (c) 2019 Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using Xunit;

using DotLang.CodeAnalysis.Syntax;

namespace DotLang.Tests
{
    using static SyntaxKind;

    public class SyntaxFactsTests
    {
        [Theory]
        [InlineData(null, None)]
        [InlineData("", None)]
        [InlineData("?", None)]
        [InlineData("NotAKeyword", None)]
        [InlineData("{", OpenBraceToken)]
        [InlineData("}", CloseBraceToken)]
        [InlineData("[", OpenBracketToken)]
        [InlineData("]", CloseBracketToken)]
        [InlineData(":", ColonToken)]
        [InlineData(";", SemicolonToken)]
        [InlineData(",", CommaToken)]
        [InlineData("->", DirectedEdgeToken)]
        [InlineData("--", UndirectedEdgeToken)]
        [InlineData("strict", StrictKeyword)]
        [InlineData("Strict", StrictKeyword)]
        [InlineData("STRICT", StrictKeyword)]
        [InlineData("graph", GraphKeyword)]
        [InlineData("Graph", GraphKeyword)]
        [InlineData("GRAPH", GraphKeyword)]
        [InlineData("digraph", DigraphKeyword)]
        [InlineData("DiGraph", DigraphKeyword)]
        [InlineData("DIGRAPH", DigraphKeyword)]
        [InlineData("subgraph", SubgraphKeyword)]
        [InlineData("SubGraph", SubgraphKeyword)]
        [InlineData("SUBGRAPH", SubgraphKeyword)]
        [InlineData("node", NodeKeyword)]
        [InlineData("Node", NodeKeyword)]
        [InlineData("NODE", NodeKeyword)]
        public void GetTextGetKind(string text, SyntaxKind kind)
        {
            Assert.Equal(kind, SyntaxFacts.GetKind(text));

            if (kind != None)
            {
                Assert.Equal(text, SyntaxFacts.GetText(kind), ignoreCase: true);
                Assert.Equal(kind, SyntaxFacts.GetKind(SyntaxFacts.GetText(kind)));
                Assert.Equal(text, SyntaxFacts.GetText(SyntaxFacts.GetKind(text)), ignoreCase: true);
            }
        }

        [Theory]
        [InlineData(null, None)]
        [InlineData("", StringLiteralToken)]
        [InlineData(" ", StringLiteralToken)]
        [InlineData("   ", StringLiteralToken)]
        [InlineData("\t", StringLiteralToken)]
        [InlineData("\n", StringLiteralToken)]
        [InlineData("\"", StringLiteralToken)]
        [InlineData("a", IdentifierToken)]
        [InlineData("a0_b", IdentifierToken)]
        [InlineData("0a0_b", StringLiteralToken)]
        [InlineData("0x0", StringLiteralToken)]
        [InlineData("0", NumeralLiteralToken)]
        [InlineData("012345", NumeralLiteralToken)]
        [InlineData(".", StringLiteralToken)]
        [InlineData("..", StringLiteralToken)]
        [InlineData("...", StringLiteralToken)]
        [InlineData(".1", NumeralLiteralToken)]
        [InlineData(".14", NumeralLiteralToken)]
        [InlineData("3.14", NumeralLiteralToken)]
        [InlineData("0003.14", NumeralLiteralToken)]
        [InlineData("_0003.14", StringLiteralToken)]
        [InlineData(".a", StringLiteralToken)]
        [InlineData(".a.b", StringLiteralToken)]
        [InlineData(".a.b.c", StringLiteralToken)]
        [InlineData(".1.", StringLiteralToken)]
        [InlineData(".1.2", StringLiteralToken)]
        [InlineData(".1.2.3", StringLiteralToken)]
        [InlineData("_", IdentifierToken)]
        [InlineData("\ufffd", IdentifierToken)]
        [InlineData("a\ufffd", IdentifierToken)]
        [InlineData("a\ufffdb", IdentifierToken)]
        [InlineData(".\ufffd", StringLiteralToken)]
        [InlineData(".\ufffd.", StringLiteralToken)]
        [InlineData("hello world", StringLiteralToken)]
        [InlineData("hello;world", StringLiteralToken)]
        [InlineData("hello,world", StringLiteralToken)]
        public void ToIdentifierToken(string id, SyntaxKind kind)
        {
            var token = SyntaxFacts.ToIdentifierToken(id);
            Assert.Equal(kind, token.Kind);
            Assert.Equal(id, token.StringValue);
        }
    }
}
