//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright (c) 2019 Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

namespace DotLang.CodeAnalysis.Syntax
{
    public enum VisitKind
    {
        Invalid,
        Enter,
        Leave
    }

    public class SyntaxVisitor
    {
        public virtual bool VisitSyntaxNode(
            SyntaxNode node,
            VisitKind visitKind)
            => true;

        public virtual void VisitSyntaxToken(SyntaxToken token)
        {
        }

        public virtual bool VisitSyntaxTree(
            SyntaxTree tree,
            VisitKind visitKind)
            => VisitSyntaxNode(tree, visitKind);

        public virtual bool VisitPreprocessorDirectiveSyntax(
            PreprocessorDirectiveSyntax directive,
            VisitKind visitKind)
            => VisitSyntaxNode(directive, visitKind);

        public virtual bool VisitNodeIdentifierSyntax(
            NodeIdentifierSyntax nodeIdentifier,
            VisitKind visitKind)
            => VisitSyntaxNode(nodeIdentifier, visitKind);

        public virtual bool VisitPortSyntax(
            PortSyntax port,
            VisitKind visitKind)
            => VisitSyntaxNode(port, visitKind);

        public virtual bool VisitAttributeSyntax(
            AttributeSyntax attribute,
            VisitKind visitKind)
            => VisitSyntaxNode(attribute, visitKind);

        public virtual bool VisitAttributeListSyntax(
            AttributeListSyntax attributeList,
            VisitKind visitKind)
            => VisitSyntaxNode(attributeList, visitKind);

        public virtual bool VisitToplevelGraphSyntax(
            ToplevelGraphSyntax toplevelGraph,VisitKind visitKind)
            => VisitSyntaxNode(toplevelGraph, visitKind);

        public virtual bool VisitGraphSyntax(
            GraphSyntax graph,
            VisitKind visitKind)
            => VisitToplevelGraphSyntax(graph, visitKind);

        public virtual bool VisitDigraphSyntax(
            DigraphSyntax digraph,
            VisitKind visitKind)
            => VisitToplevelGraphSyntax(digraph, visitKind);

        public virtual bool VisitStatementSyntax(
            StatementSyntax statement,
            VisitKind visitKind)
            => VisitSyntaxNode(statement, visitKind);

        public virtual bool VisitEmptyStatementSyntax(
            EmptyStatementSyntax emptyStatement,
            VisitKind visitKind)
            => VisitStatementSyntax(emptyStatement, visitKind);

        public virtual bool VisitNameValueStatementSyntax(
            NameValueStatementSyntax nameValueStatement,
            VisitKind visitKind)
            => VisitStatementSyntax(nameValueStatement, visitKind);

        public virtual bool VisitAttributeStatementSyntax(
            AttributeStatementSyntax attributeStatement,
            VisitKind visitKind)
            => VisitStatementSyntax(attributeStatement, visitKind);

        public virtual bool VisitNodeStatementSyntax(
            NodeStatementSyntax nodeStatement,
            VisitKind visitKind)
            => VisitStatementSyntax(nodeStatement, visitKind);

        public virtual bool VisitEdgeStatementSyntax(
            EdgeStatementSyntax edgeStatement,
            VisitKind visitKind)
            => VisitStatementSyntax(edgeStatement, visitKind);

        public virtual bool VisitSubgraphStatementSyntax(
            SubgraphStatementSyntax subgraphStatement,
            VisitKind visitKind)
            => VisitStatementSyntax(subgraphStatement, visitKind);
    }
}
