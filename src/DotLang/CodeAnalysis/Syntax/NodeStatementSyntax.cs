//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright (c) 2019 Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace DotLang.CodeAnalysis.Syntax
{
    public sealed class NodeStatementSyntax : StatementSyntax, IEdgeVertexStatementSyntax
    {
        public NodeIdentifierSyntax Identifier { get; }
        public AttributeListSyntax? Attributes { get; }

        internal NodeStatementSyntax(
            NodeIdentifierSyntax identifier,
            AttributeListSyntax? attributes,
            SyntaxToken terminatingSemicolonToken,
            bool isParsed)
            : base(terminatingSemicolonToken, isParsed)
        {
            Identifier = identifier;
            Attributes = attributes;
        }

        public NodeStatementSyntax(
            NodeIdentifierSyntax identifier)
            : this(
                identifier,
                null,
                SyntaxToken.Invalid,
                isParsed: false)
        {
        }

        public NodeStatementSyntax(
            NodeIdentifierSyntax identifier,
            AttributeListSyntax? attributes)
            : this(
                identifier,
                attributes,
                SyntaxToken.Invalid,
                isParsed: false)
        {
        }

        public NodeStatementSyntax(
            NodeIdentifierSyntax identifier,
            IEnumerable<AttributeSyntax>? attributes)
            : this(
                identifier,
                attributes is null
                    ? null
                    : new AttributeListSyntax(attributes),
                SyntaxToken.Invalid,
                isParsed: false)
        {
        }

        public NodeStatementSyntax(
            NodeIdentifierSyntax identifier,
            params AttributeSyntax[] attributes)
            : this(
                identifier,
                (IEnumerable<AttributeSyntax>)attributes)
            {
            }

        public static implicit operator NodeStatementSyntax(string identifier)
            => new NodeStatementSyntax(identifier);

        public override void Accept(SyntaxVisitor visitor)
        {
            if (visitor.VisitNodeStatementSyntax(this, VisitKind.Enter))
            {
                Identifier.Accept(visitor);
                Attributes?.Accept(visitor);
                TerminatingSemicolonToken.Accept(visitor);

                visitor.VisitNodeStatementSyntax(this, VisitKind.Leave);
            }
        }
    }
}
