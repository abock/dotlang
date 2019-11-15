//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright (c) 2019 Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace DotLang.CodeAnalysis.Syntax
{
    public sealed class SyntaxTree : SyntaxNode
    {
        public IReadOnlyList<ToplevelGraphSyntax> Graphs { get; }
        public SyntaxToken EndOfFileToken { get; }

        internal SyntaxTree(
            IReadOnlyList<ToplevelGraphSyntax> graphs,
            SyntaxToken endOfFileToken,
            bool isParsed)
            : base(isParsed)
        {
            Graphs = graphs;
            EndOfFileToken = endOfFileToken;
        }

        public SyntaxTree(ToplevelGraphSyntax graph)
            : this(
                new List<ToplevelGraphSyntax>
                {
                    graph ?? throw new ArgumentNullException(nameof(graph))
                },
                new SyntaxToken(SyntaxKind.EndOfFileToken),
                isParsed: false)
        {
        }

        public SyntaxTree(IEnumerable<ToplevelGraphSyntax>? graphs)
            : this(
                ToList(graphs),
                new SyntaxToken(SyntaxKind.EndOfFileToken),
                isParsed: false)
        {
        }

        public override void Accept(SyntaxVisitor visitor)
        {
            if (visitor.VisitSyntaxTree(this, VisitKind.Enter))
            {
                foreach (var graph in Graphs)
                {
                    graph.Accept(visitor);
                }

                EndOfFileToken.Accept(visitor);

                visitor.VisitSyntaxTree(this, VisitKind.Leave);
            }
        }
    }
}
