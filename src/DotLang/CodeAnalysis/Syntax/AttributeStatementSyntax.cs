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
    public enum AttributeKeyword
    {
        Graph,
        Edge,
        Node
    }

    public sealed class AttributeStatementSyntax : StatementSyntax
    {
        public SyntaxToken KeywordToken { get; }
        public AttributeListSyntax Attributes { get; }

        internal AttributeStatementSyntax(
            SyntaxToken keywordToken,
            AttributeListSyntax attributes,
            SyntaxToken terminatingSemicolonToken,
            bool isParsed)
            : base(terminatingSemicolonToken, isParsed)
        {
            KeywordToken = keywordToken;
            Attributes = attributes;
        }

        public AttributeStatementSyntax(
            AttributeKeyword keyword,
            AttributeListSyntax attributes)
            : this(
                new SyntaxToken(keyword switch
                {
                    AttributeKeyword.Graph => SyntaxKind.GraphKeyword,
                    AttributeKeyword.Node => SyntaxKind.NodeKeyword,
                    AttributeKeyword.Edge => SyntaxKind.EdgeKeyword,
                    _ => throw new ArgumentOutOfRangeException(nameof(keyword))
                }),
                attributes,
                SyntaxToken.Invalid,
                isParsed: false)
        {
        }

        public AttributeStatementSyntax(
            AttributeKeyword keyword,
            IEnumerable<AttributeSyntax>? attributes)
            : this(
                keyword,
                new AttributeListSyntax(attributes))
        {
        }

        public AttributeStatementSyntax(
            AttributeKeyword keyword,
            params AttributeSyntax[] attributes)
            : this(
                keyword,
                (IEnumerable<AttributeSyntax>)attributes)
        {
        }

        public override void Accept(SyntaxVisitor visitor)
        {
            if (visitor.VisitAttributeStatementSyntax(this, VisitKind.Enter))
            {
                KeywordToken.Accept(visitor);
                Attributes.Accept(visitor);
                TerminatingSemicolonToken.Accept(visitor);

                visitor.VisitAttributeStatementSyntax(this, VisitKind.Leave);
            }
        }
    }
}
