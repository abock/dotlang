//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright (c) 2019 Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;

namespace DotLang.CodeAnalysis.Syntax
{
    using static SyntaxKind;

    /// <summary>
    /// A <see cref="SyntaxVisitor"/> that produces source text from a syntax tree based
    /// on formatting settings from a <see cref="FormattedSyntaxTextWriterSettings"/>.
    /// </summary>
    public sealed class FormattedSyntaxWriter : SyntaxVisitor
    {
        readonly FormattedSyntaxTextWriter writer;
        readonly FormattedSyntaxTextWriterSettings settings;
        readonly Stack<SyntaxNode> nodeStack = new Stack<SyntaxNode>();

        public FormattedSyntaxWriter(
            TextWriter writer,
            FormattedSyntaxTextWriterSettings? settings = null)
            => this.writer = new FormattedSyntaxTextWriter(
                writer ?? throw new ArgumentNullException(nameof(writer)),
                this.settings = settings ?? new FormattedSyntaxTextWriterSettings());

        public override bool VisitSyntaxNode(
            SyntaxNode node,
            VisitKind visitKind)
        {
            if (node.IsParsed)
            {
            }

            switch (visitKind)
            {
                case VisitKind.Enter:
                    nodeStack.Push(node);
                    break;
                case VisitKind.Leave:
                    nodeStack.Pop();
                    break;
            }

            return true;
        }

        public override bool VisitSyntaxTree(
            SyntaxTree tree,
            VisitKind visitKind)
        {
            if (visitKind == VisitKind.Leave && settings.InsertFinalNewLine)
            {
                writer.WriteLine();
            }

            return true;
        }

        public override bool VisitToplevelGraphSyntax(
            ToplevelGraphSyntax toplevelGraph,
            VisitKind visitKind)
        {
            base.VisitToplevelGraphSyntax(toplevelGraph, visitKind);

            switch (visitKind)
            {
                case VisitKind.Enter:
                    writer.WriteToken(toplevelGraph.StrictKeyword);
                    writer.WriteToken(toplevelGraph.GraphTypeKeyword);
                    writer.WriteToken(toplevelGraph.IdentifierToken);
                    writer.WriteOpenBrace();
                    break;
                case VisitKind.Leave:
                    writer.WriteCloseBrace();
                    break;
            }

            return true;
        }

        bool VisitIEdgeVertexStatementSyntax(
            IEdgeVertexStatementSyntax edgeVertexStatement,
            VisitKind visitKind)
        {
            if (visitKind == VisitKind.Leave
                && nodeStack.Peek() is EdgeStatementSyntax edgeStatement
                && edgeStatement.Left == edgeVertexStatement)
            {
                writer.Write(' ');
                writer.WriteToken(edgeStatement.EdgeOperatorToken);

                return true;
            }

            return false;
        }

        public override bool VisitSubgraphStatementSyntax(
            SubgraphStatementSyntax subgraphStatement,
            VisitKind visitKind)
        {
            base.VisitSubgraphStatementSyntax(subgraphStatement, visitKind);

            switch (visitKind)
            {
                case VisitKind.Enter:
                    writer.WriteToken(subgraphStatement.GraphTypeKeyword);
                    writer.WriteToken(subgraphStatement.IdentifierToken);
                    writer.WriteOpenBrace();
                    break;
                case VisitKind.Leave:
                    writer.WriteCloseBrace();
                    if (!VisitIEdgeVertexStatementSyntax(subgraphStatement, visitKind))
                    {
                        if (settings.InsertSemicolonAfterSubgraphStatements)
                        {
                            writer.Write(';');
                        }
                        writer.WriteLine();
                    }
                    break;
            }

            return true;
        }

        public override bool VisitNodeStatementSyntax(
            NodeStatementSyntax nodeStatement,
            VisitKind visitKind)
        {
            base.VisitNodeStatementSyntax(nodeStatement, visitKind);

            if (visitKind == VisitKind.Leave)
            {
                if (!VisitIEdgeVertexStatementSyntax(nodeStatement, visitKind))
                {
                    if (settings.InsertSemicolonAfterNodeStatements)
                    {
                        writer.WriteDelimiterIfNeeded(';');
                    }

                    writer.WriteLine();
                }
            }

            return true;
        }

        public override bool VisitNodeIdentifierSyntax(
            NodeIdentifierSyntax nodeIdentifier,
            VisitKind visitKind)
        {
            base.VisitNodeIdentifierSyntax(nodeIdentifier, visitKind);

            if (visitKind == VisitKind.Enter)
            {
                writer.WriteToken(nodeIdentifier.IdentifierToken);
            }

            return true;
        }

        public override bool VisitPortSyntax(
            PortSyntax port,
            VisitKind visitKind)
        {
            base.VisitPortSyntax(port, visitKind);

            if (visitKind == VisitKind.Enter)
            {
                writer.WriteToken(port.Colon1Token);
                writer.WriteToken(port.PortOrCompassIdentifierToken);
                writer.WriteToken(port.Colon2Token);
                writer.WriteToken(port.CompassIdentifierToken);
            }

            return true;
        }

        public override bool VisitAttributeListSyntax(
            AttributeListSyntax attributeList,
            VisitKind visitKind)
        {
            base.VisitAttributeListSyntax(attributeList, visitKind);

            if (attributeList.Count > 0)
            {
                switch (visitKind)
                {
                    case VisitKind.Enter:
                        writer.WriteToken(OpenBracketToken);
                        break;
                    case VisitKind.Leave:
                        writer.WriteToken(CloseBracketToken);
                        break;
                }
            }

            return true;
        }

        public override bool VisitAttributeSyntax(
            AttributeSyntax attribute,
            VisitKind visitKind)
        {
            base.VisitAttributeSyntax(attribute, visitKind);

            switch (visitKind)
            {
                case VisitKind.Enter:
                    writer.WriteToken(attribute.NameToken);
                    writer.WriteToken(EqualsToken);
                    writer.WriteToken(attribute.ValueToken);
                    break;
                case VisitKind.Leave:
                    if (nodeStack.Peek() is AttributeListSyntax listSyntax
                        && listSyntax.Attributes[listSyntax.Attributes.Count - 1] != attribute)
                    {
                        writer.Write(", ");
                    }
                    break;
            }

            return true;
        }

        public override bool VisitAttributeStatementSyntax(
            AttributeStatementSyntax attributeStatement,
            VisitKind visitKind)
        {
            base.VisitAttributeStatementSyntax(attributeStatement, visitKind);

            switch (visitKind)
            {
                case VisitKind.Enter:
                    writer.WriteToken(attributeStatement.KeywordToken);
                    break;
                case VisitKind.Leave:
                    if (settings.InsertSemicolonAfterNodeStatements)
                    {
                        writer.WriteDelimiterIfNeeded(';');
                    }

                    writer.WriteLine();
                    break;
            }

            return true;
        }

        public override bool VisitNameValueStatementSyntax(
            NameValueStatementSyntax nameValueStatement,
            VisitKind visitKind)
        {
            base.VisitNameValueStatementSyntax(nameValueStatement, visitKind);

            switch (visitKind)
            {
                case VisitKind.Enter:
                    writer.WriteToken(nameValueStatement.NameToken);
                    writer.WriteToken(EqualsToken);
                    writer.WriteToken(nameValueStatement.ValueToken);
                    break;
                case VisitKind.Leave:
                    if (settings.InsertSemicolonAfterNodeStatements)
                    {
                        writer.WriteDelimiterIfNeeded(';');
                    }

                    writer.WriteLine();
                    break;
            }

            return true;
        }
    }
}
