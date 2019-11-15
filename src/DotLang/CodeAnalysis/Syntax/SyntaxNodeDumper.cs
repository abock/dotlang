//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright (c) 2019 Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace DotLang.CodeAnalysis.Syntax
{
    public class SyntaxNodeDumper : SyntaxVisitor
    {
        readonly List<(SyntaxNode Node, IReadOnlyList<SyntaxToken> Tokens)> nodeStack
            = new List<(SyntaxNode, IReadOnlyList<SyntaxToken>)>();

        protected IReadOnlyList<(SyntaxNode Node, IReadOnlyList<SyntaxToken> Tokens)> NodeStack
            => nodeStack;

        public sealed override bool VisitSyntaxNode(SyntaxNode node, VisitKind visitKind)
        {
            if (visitKind == VisitKind.Enter)
            {
                nodeStack.Insert(0, (node, new List<SyntaxToken>()));
            }

            if (visitKind == VisitKind.Enter || visitKind == VisitKind.Leave)
            {
                try
                {
                    return DumpNode(node, visitKind);
                }
                finally
                {
                    if (visitKind == VisitKind.Leave)
                    {
                        nodeStack.RemoveAt(0);
                    }
                }
            }

            return true;
        }

        public sealed override void VisitSyntaxToken(SyntaxToken token)
            => ((List<SyntaxToken>)nodeStack[0].Tokens).Add(token);

        protected virtual bool DumpNode(SyntaxNode node, VisitKind visitKind)
            => true;
    }
}
