//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright (c) 2019 Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace DotLang.CodeAnalysis.Syntax
{
    public sealed class SubgraphStatementSyntax : StatementSyntax, IEdgeVertexStatementSyntax
    {
        public SyntaxToken GraphTypeKeyword { get; }
        public SyntaxToken IdentifierToken { get; }
        public SyntaxToken OpenBraceToken { get; }
        public IReadOnlyList<StatementSyntax> Statements { get; }
        public SyntaxToken CloseBraceToken { get; }

        internal SubgraphStatementSyntax(
            SyntaxToken graphTypeKeyword,
            SyntaxToken identifierToken,
            SyntaxToken openBraceToken,
            IReadOnlyList<StatementSyntax> statements,
            SyntaxToken closeBraceToken,
            SyntaxToken terminatingSemicolonToken,
            bool isParsed)
            : base(terminatingSemicolonToken, isParsed)
        {
            GraphTypeKeyword = graphTypeKeyword;
            IdentifierToken = identifierToken;
            OpenBraceToken = openBraceToken;
            Statements = statements;
            CloseBraceToken = closeBraceToken;
        }

        public SubgraphStatementSyntax(
            string? identifier,
            IEnumerable<StatementSyntax>? statements = null)
            : this(
                new SyntaxToken(SyntaxKind.SubgraphKeyword),
                SyntaxFacts.ToIdentifierToken(identifier),
                new SyntaxToken(SyntaxKind.OpenBraceToken),
                ToList(statements),
                new SyntaxToken(SyntaxKind.CloseBraceToken),
                SyntaxToken.Invalid,
                isParsed: false)
        {
        }

        public SubgraphStatementSyntax(IEnumerable<StatementSyntax>? statements = null)
            : this(null, statements)
        {
        }

        public SubgraphStatementSyntax(params StatementSyntax[] statements)
            : this(null, (IEnumerable<StatementSyntax>)statements)
        {
        }

        public SubgraphStatementSyntax(
            string? identifier,
            params StatementSyntax[] statements)
            : this(identifier, (IEnumerable<StatementSyntax>)statements)
        {
        }

        public override void Accept(SyntaxVisitor visitor)
        {
            if (visitor.VisitSubgraphStatementSyntax(this, VisitKind.Enter))
            {
                GraphTypeKeyword.Accept(visitor);
                IdentifierToken.Accept(visitor);
                OpenBraceToken.Accept(visitor);

                foreach (var statement in Statements)
                {
                    statement.Accept(visitor);
                }

                CloseBraceToken.Accept(visitor);
                TerminatingSemicolonToken.Accept(visitor);

                visitor.VisitSubgraphStatementSyntax(this, VisitKind.Leave);
            }
        }
    }
}
