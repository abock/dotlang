//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright (c) 2019 Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace DotLang.CodeAnalysis.Syntax
{
    public sealed class GraphSyntax : ToplevelGraphSyntax
    {
        internal GraphSyntax(
            SyntaxToken strictKeyword,
            SyntaxToken graphTypeKeyword,
            SyntaxToken identifierToken,
            SyntaxToken openBraceToken,
            IReadOnlyList<StatementSyntax> statements,
            SyntaxToken closeBraceToken,
            bool isParsed)
            : base(
                strictKeyword,
                graphTypeKeyword,
                identifierToken,
                openBraceToken,
                statements,
                closeBraceToken,
                isParsed)
        {
        }

        public GraphSyntax(
            string? identifier = null,
            bool isStrict = false,
            IEnumerable<StatementSyntax>? statements = null)
            : this(
                new SyntaxToken(isStrict
                    ? SyntaxKind.StrictKeyword
                    : SyntaxKind.None),
                new SyntaxToken(SyntaxKind.GraphKeyword),
                SyntaxFacts.ToIdentifierToken(identifier),
                new SyntaxToken(SyntaxKind.OpenBraceToken),
                ToList(statements),
                new SyntaxToken(SyntaxKind.CloseBraceToken),
                isParsed: false)
        {
        }

        public GraphSyntax(
            string? identifier = null,
            bool isStrict = false,
            params StatementSyntax[] statements)
            : this(
                identifier,
                isStrict,
                (IEnumerable<StatementSyntax>)statements)
        {
        }

        public override void Accept(SyntaxVisitor visitor)
        {
            if (visitor.VisitGraphSyntax(this, VisitKind.Enter))
            {
                base.Accept(visitor);

                visitor.VisitGraphSyntax(this, VisitKind.Leave);
            }
        }
    }
}
