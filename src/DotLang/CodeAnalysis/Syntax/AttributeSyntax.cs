//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright (c) 2019 Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

namespace DotLang.CodeAnalysis.Syntax
{
    public sealed class AttributeSyntax : SyntaxNode, INameValueSyntax
    {
        public SyntaxToken NameToken { get; }
        public SyntaxToken EqualsToken { get; }
        public SyntaxToken ValueToken { get; }
        public SyntaxToken SeparatorToken { get; }

        internal AttributeSyntax(
            SyntaxToken nameToken,
            SyntaxToken equalsToken,
            SyntaxToken valueToken,
            SyntaxToken separatorToken,
            bool isParsed)
            : base(isParsed)
        {
            NameToken = nameToken;
            EqualsToken = equalsToken;
            ValueToken = valueToken;
            SeparatorToken = separatorToken;
        }

        public AttributeSyntax(string name, string value)
            : this((name, value))
        {
        }

        public AttributeSyntax((string Name, string Value) attribute)
            : this(
                SyntaxFacts.ToIdentifierToken(attribute.Name),
                new SyntaxToken(SyntaxKind.EqualsToken),
                SyntaxFacts.ToIdentifierToken(attribute.Value),
                new SyntaxToken(SyntaxKind.CommaToken),
                isParsed: false)
        {
        }

        public static implicit operator AttributeSyntax((string Name, string Value) attribute)
            => new AttributeSyntax(attribute);

        public override void Accept(SyntaxVisitor visitor)
        {
            if (visitor.VisitAttributeSyntax(this, VisitKind.Enter))
            {
                NameToken.Accept(visitor);
                EqualsToken.Accept(visitor);
                ValueToken.Accept(visitor);
                SeparatorToken.Accept(visitor);

                visitor.VisitAttributeSyntax(this, VisitKind.Leave);
            }
        }
    }
}
