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
    public class SyntaxTreeTests
    {
        [Fact]
        public void EmptyAnonymousGraph()
            => AssertSourceText(
                new GraphSyntax(),
                "graph {\n}");

        [Fact]
        public void EmptyAnonymousDigraph()
            => AssertSourceText(
                new DigraphSyntax(),
                "digraph {\n}");

        [Fact]
        public void EmptyStrictAnonymousGraph()
            => AssertSourceText(
                new GraphSyntax(isStrict: true),
                "strict graph {\n}");

        [Fact]
        public void EmptyStrictAnonymousDigraph()
            => AssertSourceText(
                new DigraphSyntax(isStrict: true),
                "strict digraph {\n}");

        [Fact]
        public void EmptyGraph()
            => AssertSourceText(
                new GraphSyntax("X"),
                "graph X {\n}");

        [Fact]
        public void EmptyDigraph()
            => AssertSourceText(
                new DigraphSyntax("X"),
                "digraph X {\n}");

        [Fact]
        public void EmptyStrictGraph()
            => AssertSourceText(
                new GraphSyntax("X", isStrict: true),
                "strict graph X {\n}");

        [Fact]
        public void EmptyStrictDigraph()
            => AssertSourceText(
                new DigraphSyntax("X", isStrict: true),
                "strict digraph X {\n}");

        [Fact]
        public void EmptySubgraph()
            => AssertSourceText(
                new GraphSyntax(statements: new[]
                {
                    new SubgraphStatementSyntax()
                }),
                @"
                    graph {
                        subgraph {
                        }
                    }
                ");

        [Fact]
        public void EmptySubgraphs()
            => AssertSourceText(
                new GraphSyntax(statements: new[]
                {
                    new SubgraphStatementSyntax(),
                    new SubgraphStatementSyntax(),
                    new SubgraphStatementSyntax(),
                }),
                @"
                    graph {
                        subgraph {
                        }
                        subgraph {
                        }
                        subgraph {
                        }
                    }
                ");

        [Fact]
        public void EmptyNestedSubgraphs()
            => AssertSourceText(
                new GraphSyntax("Hello", isStrict: true, statements: new[]
                {
                    new SubgraphStatementSyntax(),
                    new SubgraphStatementSyntax(statements: new[]
                    {
                        new SubgraphStatementSyntax(statements: new[]
                        {
                            new SubgraphStatementSyntax("Sub")
                        })
                    }),
                    new SubgraphStatementSyntax(),
                }),
                @"
                    strict graph Hello {
                        subgraph {
                        }
                        subgraph {
                            subgraph {
                                subgraph Sub {
                                }
                            }
                        }
                        subgraph {
                        }
                    }
                ");

        [Fact]
        public void SimpleNodes()
        {
            AssertSourceText(
                new GraphSyntax(statements: new[]
                {
                    new NodeStatementSyntax("A"),
                    new NodeStatementSyntax("B"),
                    new NodeStatementSyntax("C")
                }),
                @"
                    graph {
                        A;
                        B;
                        C;
                    }
                "
            );
        }

        [Fact]
        public void EdgeNodes()
        {
            AssertSourceText(
                new DigraphSyntax(statements: new StatementSyntax[]
                {
                    new EdgeStatementSyntax(
                        new NodeStatementSyntax("A"),
                        EdgeKind.Directed,
                        new EdgeStatementSyntax(
                            new NodeStatementSyntax("B"),
                            EdgeKind.Directed,
                            new NodeStatementSyntax("C"))),
                    new EdgeStatementSyntax(
                        new NodeStatementSyntax("V"),
                        EdgeKind.Directed,
                        new EdgeStatementSyntax(
                            new SubgraphStatementSyntax(statements: new[]
                            {
                                new NodeStatementSyntax("W"),
                                new NodeStatementSyntax("X"),
                                new NodeStatementSyntax("Y")
                            }),
                            EdgeKind.Directed,
                            new EdgeStatementSyntax(
                                new NodeStatementSyntax("Z"),
                                EdgeKind.Directed,
                                new NodeStatementSyntax("A")
                            )))
                }),
                @"
                    digraph {
                        A -> B -> C;
                        V -> subgraph {
                            W;
                            X;
                            Y;
                        } -> Z -> A;
                    }
                "
            );
        }

        void AssertSourceText(SyntaxNode node, string expectedSourceText)
            => Assert.Equal(
                TestHelpers.TrimCommonLeadingWhiteSpace(expectedSourceText),
                node.ToString());
    }
}
