//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright (c) 2019 Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace DotLang.CodeAnalysis.Syntax
{
    public sealed class DigraphSyntax : ToplevelGraphSyntax
    {
        internal DigraphSyntax(
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

        public DigraphSyntax(
            string? identifier = null,
            bool isStrict = false,
            IEnumerable<StatementSyntax>? statements = null)
            : this(
                new SyntaxToken(isStrict ? SyntaxKind.StrictKeyword : SyntaxKind.None),
                new SyntaxToken(SyntaxKind.DigraphKeyword),
                SyntaxFacts.ToIdentifierToken(identifier),
                new SyntaxToken(SyntaxKind.OpenBraceToken),
                ToList(statements),
                new SyntaxToken(SyntaxKind.CloseBraceToken),
                isParsed: false)
        {
        }

        public DigraphSyntax(
            string? identifier = null,
            bool isStrict = false,
            params StatementSyntax[] statements)
            : this(
                identifier,
                isStrict,
                (IEnumerable<StatementSyntax>)statements)
        {
        }

        public DigraphSyntax(IEnumerable<StatementSyntax> statements)
            : this(
                identifier: null,
                isStrict: false,
                (IEnumerable<StatementSyntax>)statements)
        {
        }

        public override void Accept(SyntaxVisitor visitor)
        {
            if (visitor.VisitDigraphSyntax(this, VisitKind.Enter))
            {
                base.Accept(visitor);

                visitor.VisitDigraphSyntax(this, VisitKind.Leave);
            }
        }
    }
}
