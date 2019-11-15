//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright (c) 2019 Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace DotLang.CodeAnalysis.Syntax
{
    public abstract class ToplevelGraphSyntax : SyntaxNode
    {
        public SyntaxToken StrictKeyword { get; }
        public SyntaxToken GraphTypeKeyword { get; }
        public SyntaxToken IdentifierToken { get; }
        public SyntaxToken OpenBraceToken { get; }
        public IReadOnlyList<StatementSyntax> Statements { get; }
        public SyntaxToken CloseBraceToken { get; }

        private protected ToplevelGraphSyntax(
            SyntaxToken strictKeyword,
            SyntaxToken graphTypeKeyword,
            SyntaxToken identifierToken,
            SyntaxToken openBraceToken,
            IReadOnlyList<StatementSyntax> statements,
            SyntaxToken closeBraceToken,
            bool isParsed)
            : base(isParsed)
        {
            StrictKeyword = strictKeyword;
            GraphTypeKeyword = graphTypeKeyword;
            IdentifierToken = identifierToken;
            OpenBraceToken = openBraceToken;
            Statements = statements;
            CloseBraceToken = closeBraceToken;
        }

        public override void Accept(SyntaxVisitor visitor)
        {
            StrictKeyword.Accept(visitor);
            GraphTypeKeyword.Accept(visitor);
            IdentifierToken.Accept(visitor);
            OpenBraceToken.Accept(visitor);

            foreach (var statement in Statements)
            {
                statement.Accept(visitor);
            }

            CloseBraceToken.Accept(visitor);
        }
    }
}
