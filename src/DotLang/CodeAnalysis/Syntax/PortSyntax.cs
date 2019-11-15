//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright (c) 2019 Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

namespace DotLang.CodeAnalysis.Syntax
{
    public sealed class PortSyntax : SyntaxNode
    {
        public static PortSyntax? Create(string? portIdentifier, string? compassIdentifier)
        {
            if (portIdentifier is null && compassIdentifier is null)
            {
                return null;
            }

            return new PortSyntax(
                portIdentifier is null
                    ? SyntaxToken.Invalid
                    : new SyntaxToken(SyntaxKind.ColonToken),
                SyntaxFacts.ToIdentifierToken(portIdentifier),
                compassIdentifier is null
                    ? SyntaxToken.Invalid
                    : new SyntaxToken(SyntaxKind.ColonToken),
                SyntaxFacts.ToIdentifierToken(compassIdentifier),
                isParsed: false);
        }

        public SyntaxToken Colon1Token { get; }
        public SyntaxToken PortOrCompassIdentifierToken { get; }
        public SyntaxToken Colon2Token { get; }
        public SyntaxToken CompassIdentifierToken { get; }

        internal PortSyntax(
            SyntaxToken colonToken1,
            SyntaxToken portOrCompassIdentifierToken,
            SyntaxToken colonToken2,
            SyntaxToken compassIdentifierToken,
            bool isParsed)
            : base(isParsed)
        {
            Colon1Token = colonToken1;
            PortOrCompassIdentifierToken = portOrCompassIdentifierToken;
            Colon2Token = colonToken2;
            CompassIdentifierToken = compassIdentifierToken;
        }

        public override void Accept(SyntaxVisitor visitor)
        {
            if (visitor.VisitPortSyntax(this, VisitKind.Enter))
            {
                Colon1Token.Accept(visitor);
                PortOrCompassIdentifierToken.Accept(visitor);
                Colon2Token.Accept(visitor);
                CompassIdentifierToken.Accept(visitor);

                visitor.VisitPortSyntax(this, VisitKind.Leave);
            }
        }
    }
}
