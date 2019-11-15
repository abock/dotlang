//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright (c) 2019 Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

namespace DotLang.CodeAnalysis.Syntax
{
    public sealed class NameValueStatementSyntax : StatementSyntax, INameValueSyntax
    {
        public SyntaxToken NameToken { get; }
        public SyntaxToken EqualsToken { get; }
        public SyntaxToken ValueToken { get; }

        internal NameValueStatementSyntax(
            SyntaxToken nameToken,
            SyntaxToken equalsToken,
            SyntaxToken valueToken,
            SyntaxToken terminatingSemicolonToken,
            bool isParsed)
            : base(terminatingSemicolonToken, isParsed)
        {
            NameToken = nameToken;
            EqualsToken = equalsToken;
            ValueToken = valueToken;
        }

        public NameValueStatementSyntax((string Name, string Value) nameValue)
            : this(
                SyntaxFacts.ToIdentifierToken(nameValue.Name),
                new SyntaxToken(SyntaxKind.EqualsToken),
                SyntaxFacts.ToIdentifierToken(nameValue.Value),
                SyntaxToken.Invalid,
                isParsed: false)
        {
        }

        public NameValueStatementSyntax(string name, string value)
            : this((name, value))
        {
        }

        public static implicit operator NameValueStatementSyntax((string Name, string Value) nameValue)
            => new NameValueStatementSyntax(nameValue);

        public override void Accept(SyntaxVisitor visitor)
        {
            if (visitor.VisitNameValueStatementSyntax(this, VisitKind.Enter))
            {
                NameToken.Accept(visitor);
                EqualsToken.Accept(visitor);
                ValueToken.Accept(visitor);
                TerminatingSemicolonToken.Accept(visitor);

                visitor.VisitNameValueStatementSyntax(this, VisitKind.Leave);
            }
        }
    }
}
