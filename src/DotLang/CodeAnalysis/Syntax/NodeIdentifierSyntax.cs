//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright (c) 2019 Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

namespace DotLang.CodeAnalysis.Syntax
{
    public sealed class NodeIdentifierSyntax : SyntaxNode
    {
        public SyntaxToken IdentifierToken { get; }
        public PortSyntax? Port { get; }

        internal NodeIdentifierSyntax(
            SyntaxToken identifierToken,
            PortSyntax? port,
            bool isParsed)
            : base(isParsed)
        {
            IdentifierToken = identifierToken;
            Port = port;
        }

        public NodeIdentifierSyntax(
            string identifier,
            string? portIdentifier = null,
            string? compassIdentifier = null)
            : this(
                SyntaxFacts.ToIdentifierToken(identifier),
                PortSyntax.Create(portIdentifier, compassIdentifier),
                isParsed: false)
        {
        }

        public static implicit operator NodeIdentifierSyntax(string identifier)
            => new NodeIdentifierSyntax(identifier);

        public static implicit operator NodeIdentifierSyntax((
            string Identifier,
            string PortIdentifier) id)
            => new NodeIdentifierSyntax(
                id.Identifier,
                id.PortIdentifier);

        public static implicit operator NodeIdentifierSyntax((
            string Identifier,
            string PortIdentifier,
            string CompassIdentifier) id)
            => new NodeIdentifierSyntax(
                id.Identifier,
                id.PortIdentifier,
                id.CompassIdentifier);

        public override void Accept(SyntaxVisitor visitor)
        {
            if (visitor.VisitNodeIdentifierSyntax(this, VisitKind.Enter))
            {
                IdentifierToken.Accept(visitor);
                Port?.Accept(visitor);

                visitor.VisitNodeIdentifierSyntax(this, VisitKind.Leave);
            }
        }
    }
}
