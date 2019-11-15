//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright (c) 2019 Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System.Collections;
using System.Collections.Generic;

namespace DotLang.CodeAnalysis.Syntax
{
    public sealed class AttributeListSyntax : SyntaxNode, IReadOnlyList<AttributeSyntax>
    {
        public SyntaxToken OpenBracketToken { get; }
        public IReadOnlyList<AttributeSyntax> Attributes { get; }
        public SyntaxToken CloseBracketToken { get; }

        internal AttributeListSyntax(
            SyntaxToken openBracketToken,
            IReadOnlyList<AttributeSyntax> attributes,
            SyntaxToken closeBracketToken,
            bool isParsed)
            : base(isParsed)
        {
            OpenBracketToken = openBracketToken;
            Attributes = attributes;
            CloseBracketToken = closeBracketToken;
        }

        public AttributeListSyntax(IEnumerable<AttributeSyntax>? attributes)
            : this (
                SyntaxToken.Invalid,
                ToList(attributes),
                SyntaxToken.Invalid,
                isParsed: false)
        {
        }

        public override void Accept(SyntaxVisitor visitor)
        {
            if (visitor.VisitAttributeListSyntax(this, VisitKind.Enter))
            {
                OpenBracketToken.Accept(visitor);

                foreach (var attribute in Attributes)
                {
                    attribute.Accept(visitor);
                }

                CloseBracketToken.Accept(visitor);

                visitor.VisitAttributeListSyntax(this, VisitKind.Leave);
            }
        }

        public int Count
            => Attributes.Count;

        public AttributeSyntax this[int index]
            => Attributes[index];

        public IEnumerator<AttributeSyntax> GetEnumerator()
            => Attributes.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
