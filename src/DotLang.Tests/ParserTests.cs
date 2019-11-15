//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright (c) 2019 Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System;

using Xunit;

using DotLang.CodeAnalysis.Syntax;

namespace DotLang.Tests
{
    public class ParserTests
    {
        [Theory]
        [InlineData("graph{}")]
        [InlineData("digraph{}")]
        [InlineData("digraph{}graph{}")]
        [InlineData("strict graph { }")]
        [InlineData("strict graph A { }")]
        [InlineData("strict graph A { } strict digraph Hello {  \n  }    \n\n")]
        [InlineData("graph { ; }")]
        [InlineData("graph { ; ; }")]
        [InlineData("graph { ; ; ; }")]
        [InlineData("graph{;\t\t\t;;;;;     \t\t\r    \n \n \n }")]
        [InlineData("graph{;;;;;;;;;}")]
        public void EmptyGraphSyntax(string sourceText)
            => AssertRoundTrip(sourceText);

        [Theory]
        [InlineData("graph { a = b }")]
        [InlineData("graph { aaa=bbb c=d }")]
        [InlineData("graph { message=\"hello world\" ; { __key = __value } }")]
        public void NameValueStatementSyntax(string sourceText)
            => AssertRoundTrip(sourceText);

        [Theory]
        [InlineData("graph { node [a=b] }")]
        [InlineData("graph { edge [a=b, aaa = bbb ] }")]
        [InlineData("graph { graph [a=b ; aaa = bbb ; \"hello\" = \"world\" ] }")]
        [InlineData("graph{node[x=y k=v]edge[a=b]}")]
        [InlineData("graph{node[x=y;k=v]edge[a=b]}")]
        [InlineData("graph{node[x=y,k=v]edge[a=b]a->b}")]
        public void AttributeStatementSytnax(string sourceText)
            => AssertRoundTrip(sourceText);

        [Theory]
        [InlineData("digraph{a}")]
        [InlineData("graph { n0   }")]
        [InlineData("graph { a b c }")]
        [InlineData("digraph { a -> subgraph b { b0 b1 } -> c }")]
        [InlineData("graph { a[k=v] }")]
        [InlineData("graph { a[k1=v1 k2=v2] b[] c[a=b,c=d,e=f]}")]
        [InlineData("graph { a[k1=v1 k2=v2] -> b[] -> c[a=b,c=d,e=f] [edge=attrs]}")]
        public void NodeStatementSyntax(string sourceText)
            => AssertRoundTrip(sourceText);

        [Theory]
        [InlineData("digraph { a -> b }")]
        [InlineData("digraph { a -> b -> c }")]
        [InlineData("digraph { a -> b -> c -> d }")]
        [InlineData("graph { a -- b }")]
        [InlineData("graph { a -- b -- c }")]
        [InlineData("graph { a -- b -- c -- d }")]
        [InlineData("digraph { a -> b c -> d }")]
        [InlineData("digraph { a -> b ; c -> d }")]
        [InlineData("digraph { { a ; b } -> { c ; d } -> e -> { f } }")]
        public void EdgeStatementSyntax(string sourceText)
            => AssertRoundTrip(sourceText);

        [Fact]
        public void NodeStatementSyntaxTrees()
        {
            Assert.Collection(
                AssertRoundTrip("graph { }").Graphs,
                graph => Assert.Empty(graph.Statements));

            Assert.Collection(
                AssertRoundTrip("graph { a }").Graphs,
                graph => Assert.Collection(
                    graph.Statements,
                    statement => AssertNodeStatementSyntax(statement, "a")));

            Assert.Collection(
                AssertRoundTrip(@"graph {
                    a
                    bb; ccc;
                    dddd eeeee
                }").Graphs,
                graph => Assert.Collection(
                    graph.Statements,
                    statement => AssertNodeStatementSyntax(statement, "a"),
                    statement => AssertNodeStatementSyntax(statement, "bb"),
                    statement => AssertNodeStatementSyntax(statement, "ccc"),
                    statement => AssertNodeStatementSyntax(statement, "dddd"),
                    statement => AssertNodeStatementSyntax(statement, "eeeee")));
        }

        void AssertNodeStatementSyntax(
            StatementSyntax statementSyntax,
            string expectedNodeIdentifier)
            => Assert.Equal(
                expectedNodeIdentifier,
                Assert.IsType<NodeStatementSyntax>(statementSyntax)
                    .Identifier
                    .IdentifierToken
                    .StringValue);

        [Fact]
        public void SubgraphStatementSyntaxTrees()
        {
            Assert.Collection(
                AssertRoundTrip("graph { subgraph { } }").Graphs,
                graph => Assert.Collection(
                    graph.Statements,
                    statement => Assert.IsType<SubgraphStatementSyntax>(statement)));

            Assert.Collection(
                AssertRoundTrip(@"graph {
                    a subgraph b {
                        b_a subgraph b_b { b_b_c }
                    }c;d
                }").Graphs,
                graph => Assert.Collection(
                    graph.Statements,
                    statement => AssertNodeStatementSyntax(statement, "a"),
                    statement => AssertSubgraphStatementSyntax(
                        statement,
                        "b",
                        subStatement => AssertNodeStatementSyntax(subStatement, "b_a"),
                        subStatement => AssertSubgraphStatementSyntax(
                            subStatement,
                            "b_b",
                            subSubStatement => AssertNodeStatementSyntax(subSubStatement, "b_b_c"))),
                    statement => AssertNodeStatementSyntax(statement, "c"),
                    statement => AssertNodeStatementSyntax(statement, "d")));
        }

        void AssertSubgraphStatementSyntax(
            StatementSyntax statementSyntax,
            string expectedNodeIdentifier,
            params Action<StatementSyntax>[] subStatementInspectors)
        {
            var subgraphSyntax = Assert.IsType<SubgraphStatementSyntax>(statementSyntax);
            AssertIdentifier(expectedNodeIdentifier, subgraphSyntax.IdentifierToken);
            Assert.Collection(subgraphSyntax.Statements, subStatementInspectors);
        }

        void AssertIdentifier(string expected, SyntaxToken actual)
        {
            Assert.True(actual.IsValid());
            Assert.True(actual.IsIdentifier());
            Assert.Equal(expected, actual.StringValue);
        }

        // [Theory]
        // [InlineData("{a}")]
        // [InlineData("{")]
        // [InlineData("{a")]
        // [InlineData("graph{a")]
        // public void InvalidSyntax(string sourceText)
        //    => Assert.ThrowsAny<Exception>(() => AssertRoundTrip(sourceText));

        SyntaxTree AssertRoundTrip(string sourceText)
        {
            var syntaxTree = new Parser(sourceText).Parse();
            syntaxTree.Accept(new AssertTokensHaveSourceText());
            Assert.Equal(sourceText, syntaxTree.ToString());
            return syntaxTree;
        }

        sealed class AssertTokensHaveSourceText : SyntaxVisitor
        {
            public override void VisitSyntaxToken(SyntaxToken token)
            {
                if (token.Kind != SyntaxKind.EndOfFileToken)
                {
                    Assert.NotEqual(SyntaxKind.None, token.Kind);
                    Assert.NotEmpty(token.SourceText);
                }
            }
        }
    }
}
