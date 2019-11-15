//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright (c) 2019 Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System;

namespace DotLang.CodeAnalysis.Syntax
{
    public enum EdgeKind
    {
        Directed,
        Undirected
    }

    public sealed class EdgeStatementSyntax : StatementSyntax, IEdgeVertexStatementSyntax
    {
        public IEdgeVertexStatementSyntax Left { get; }
        public SyntaxToken EdgeOperatorToken { get; }
        public IEdgeVertexStatementSyntax Right { get; }
        public AttributeListSyntax? Attributes { get; }

        internal EdgeStatementSyntax(
            IEdgeVertexStatementSyntax left,
            SyntaxToken edgeOperatorToken,
            IEdgeVertexStatementSyntax right,
            AttributeListSyntax? attributes,
            SyntaxToken terminatingSemicolonToken,
            bool isParsed)
            : base(terminatingSemicolonToken, isParsed)
        {
            Left = left;
            EdgeOperatorToken = edgeOperatorToken;
            Right = right;
            Attributes = attributes;
        }

        public EdgeStatementSyntax(
            IEdgeVertexStatementSyntax left,
            EdgeKind edgeKind,
            IEdgeVertexStatementSyntax right,
            AttributeListSyntax? attributes = null)
            : this(
                left,
                new SyntaxToken(edgeKind switch
                {
                    EdgeKind.Directed => SyntaxKind.DirectedEdgeToken,
                    EdgeKind.Undirected => SyntaxKind.UndirectedEdgeToken,
                    _ => throw new ArgumentOutOfRangeException(nameof(edgeKind))
                }),
                right,
                attributes,
                SyntaxToken.Invalid,
                isParsed: false)
        {
        }

        public EdgeStatementSyntax(
            string left,
            EdgeKind edgeKind,
            IEdgeVertexStatementSyntax right,
            AttributeListSyntax? attributes = null)
            : this(
                new NodeStatementSyntax(left),
                edgeKind,
                right,
                attributes)
        {
        }

        public EdgeStatementSyntax(
            IEdgeVertexStatementSyntax left,
            EdgeKind edgeKind,
            string right,
            AttributeListSyntax? attributes = null)
            : this(
                left,
                edgeKind,
                new NodeStatementSyntax(right),
                attributes)
        {
        }

        public EdgeStatementSyntax(
            string left,
            EdgeKind edgeKind,
            string right,
            AttributeListSyntax? attributes = null)
            : this(
                new NodeStatementSyntax(left),
                edgeKind,
                new NodeStatementSyntax(right),
                attributes)
        {
        }

        public override void Accept(SyntaxVisitor visitor)
        {
            if (visitor.VisitEdgeStatementSyntax(this, VisitKind.Enter))
            {
                Left.Accept(visitor);
                EdgeOperatorToken.Accept(visitor);
                Right.Accept(visitor);
                Attributes?.Accept(visitor);
                TerminatingSemicolonToken.Accept(visitor);

                visitor.VisitEdgeStatementSyntax(this, VisitKind.Leave);
            }
        }
    }
}
