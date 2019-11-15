//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright (c) 2019 Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

namespace DotLang.CodeAnalysis.Syntax
{
    public sealed class EmptyStatementSyntax : StatementSyntax
    {
        internal EmptyStatementSyntax(
            SyntaxToken terminatingSemicolonToken,
            bool isParsed)
            : base(terminatingSemicolonToken, isParsed)
        {
        }

        public EmptyStatementSyntax() : this(SyntaxToken.Invalid, isParsed: false)
        {
        }

        public override void Accept(SyntaxVisitor visitor)
        {
            if (visitor.VisitEmptyStatementSyntax(this, VisitKind.Enter))
            {
                TerminatingSemicolonToken.Accept(visitor);

                visitor.VisitEmptyStatementSyntax(this, VisitKind.Leave);
            }
        }
    }
}
