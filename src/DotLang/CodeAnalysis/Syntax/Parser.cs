//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright (c) 2019 Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;

using DotLang.CodeAnalysis.Text;

namespace DotLang.CodeAnalysis.Syntax
{
    using static SyntaxKind;

    /// <summary>
    /// Consumes <see cref="SyntaxToken"/> from <see cref="Lexer"/> to create
    /// an abstract syntax tree of <see cref="SyntaxNode"/>.
    /// </summary>
    public sealed class Parser
    {
        readonly Lexer lexer;

        public Parser(string sourceText)
            : this(new Lexer(sourceText))
        {
        }

        public Parser(SourceText sourceText)
            : this(new Lexer(sourceText))
        {
        }

        public Parser(Lexer lexer)
        {
            this.lexer = lexer
                ?? throw new ArgumentNullException(nameof(lexer));
        }

        public SyntaxTree Parse()
        {
            List<ToplevelGraphSyntax>? graphs = null;

            while (true)
            {
                if (lexer.PeekToken().Kind == EndOfFileToken)
                {
                    return new SyntaxTree(
                        ListHelper.ListOrEmpty(graphs),
                        lexer.ReadToken(),
                        isParsed: true);
                }

                ListHelper.Append(ref graphs, ParseGraphSyntax());
            }

            throw ShouldNotBeReached();
        }

        ToplevelGraphSyntax ParseGraphSyntax()
        {
            SyntaxToken strictKeyword = SyntaxToken.Invalid;
            SyntaxToken graphTypeKeyword = SyntaxToken.Invalid;
            SyntaxToken identifierToken = SyntaxToken.Invalid;
            SyntaxToken openBraceToken = SyntaxToken.Invalid;

            var token = Expect(
                StrictKeyword,
                GraphKeyword,
                DigraphKeyword);

            switch (token.Kind)
            {
                case StrictKeyword:
                    strictKeyword = token;
                    graphTypeKeyword = Expect(GraphKeyword, DigraphKeyword);
                    break;
                default:
                    graphTypeKeyword = token;
                    break;
            }

            token = ExpectIdentifierOr(OpenBraceToken);
            switch (token.Kind)
            {
                case OpenBraceToken:
                    openBraceToken = token;
                    break;
                default:
                    if (token.IsIdentifier())
                    {
                        identifierToken = token;
                        openBraceToken = Expect(OpenBraceToken);
                    }
                    else
                    {
                        throw ShouldNotBeReached();
                    }
                    break;
            }

            var (statements, closeBraceToken) = ParseStatementListSyntax();

            if (graphTypeKeyword.Kind == GraphKeyword)
            {
                return new GraphSyntax(
                    strictKeyword,
                    graphTypeKeyword,
                    identifierToken,
                    openBraceToken,
                    statements,
                    closeBraceToken,
                    isParsed: true);
            }
            else
            {
                return new DigraphSyntax(
                    strictKeyword,
                    graphTypeKeyword,
                    identifierToken,
                    openBraceToken,
                    statements,
                    closeBraceToken,
                    isParsed: true);
            }
        }

        SubgraphStatementSyntax ParseSubgraphStatementSyntax(SyntaxToken token)
        {
            Debug.Assert(token.Kind == SubgraphKeyword || token.Kind == OpenBraceToken);

            SyntaxToken graphTypeKeyword = SyntaxToken.Invalid;
            SyntaxToken identifierToken = SyntaxToken.Invalid;
            SyntaxToken openBraceToken = SyntaxToken.Invalid;

            switch (token.Kind)
            {
                case SubgraphKeyword:
                    graphTypeKeyword = token;
                    token = ExpectIdentifierOr(OpenBraceToken);
                    if (token.IsIdentifier())
                    {
                        identifierToken = token;
                        openBraceToken = Expect(OpenBraceToken);
                    }
                    else
                    {
                        openBraceToken = token;
                    }
                    break;
                case OpenBraceToken:
                    openBraceToken = token;
                    break;
                default:
                    throw ShouldNotBeReached();
            }

            var (statements, closeBraceToken) = ParseStatementListSyntax();
            return new SubgraphStatementSyntax(
                graphTypeKeyword,
                identifierToken,
                openBraceToken,
                statements,
                closeBraceToken,
                ReadOptionalSemicolonToken(),
                isParsed: true);
        }

        (IReadOnlyList<StatementSyntax> Statements, SyntaxToken CloseBraceToken) ParseStatementListSyntax()
        {
            List<StatementSyntax>? statements = null;

            while (true)
            {
                var token = lexer.PeekToken();
                if (token.Kind == CloseBraceToken)
                {
                    return (
                        ListHelper.ListOrEmpty(statements),
                        lexer.ReadToken());
                }

                ListHelper.Append(ref statements, ParseStatementSyntax());
            }

            throw ShouldNotBeReached();
        }

        StatementSyntax ParseStatementSyntax()
        {
            var token = ExpectIdentifierOr(
                SubgraphKeyword,
                OpenBraceToken,
                GraphKeyword,
                NodeKeyword,
                EdgeKeyword,
                SemicolonToken);

            switch (token.Kind)
            {
                case SubgraphKeyword:
                case OpenBraceToken:
                    return ParsePossibleEdgeStatementSyntax(
                        ParseSubgraphStatementSyntax(token));
                case GraphKeyword:
                case NodeKeyword:
                case EdgeKeyword:
                    return new AttributeStatementSyntax(
                        token,
                        ParseAttributeListSyntax(),
                        ReadOptionalSemicolonToken(),
                        isParsed: true);
                case SemicolonToken:
                    return new EmptyStatementSyntax(
                        token,
                        isParsed: true);
                default:
                    Debug.Assert(token.IsIdentifier());

                    if (lexer.PeekToken().Kind == EqualsToken)
                    {
                        return new NameValueStatementSyntax(
                            token,
                            lexer.ReadToken(),
                            ExpectIdentifier(),
                            ReadOptionalSemicolonToken(),
                            isParsed: true);
                    }

                    return ParsePossibleEdgeStatementSyntax(
                        ParseNodeStatementSyntax(token));
            }
        }

        StatementSyntax ParsePossibleEdgeStatementSyntax(StatementSyntax lhsStatementSyntax)
        {
            if (!lexer.PeekToken().IsEdge() ||
                !(lhsStatementSyntax is IEdgeVertexStatementSyntax lhsEdgeVertex))
            {
                return lhsStatementSyntax;
            }

            var edgeOperatorToken = lexer.ReadToken();
            var rhs = ParseStatementSyntax();

            if (rhs is IEdgeVertexStatementSyntax rhsEdgeVertex)
            {
                return new EdgeStatementSyntax(
                    lhsEdgeVertex,
                    edgeOperatorToken,
                    rhsEdgeVertex,
                    ParseOptionalAttributeListSyntax(),
                    ReadOptionalSemicolonToken(),
                    isParsed: true);
            }

            throw ShouldNotBeReached($"Expected an IEdgeVertext SyntaxNode, got {rhs?.GetType().ToString() ?? "(null)"}");
        }

        NodeIdentifierSyntax ParseNodeIdentifierSyntax(SyntaxToken token)
        {
            Debug.Assert(token.IsIdentifier());
            return new NodeIdentifierSyntax(
                token,
                ParsePortSyntax(),
                isParsed: true);
        }

        PortSyntax? ParsePortSyntax()
        {
            if (lexer.PeekToken().Kind != ColonToken)
            {
                return null;
            }

            var colon1Token = Expect(ColonToken);
            var portOrCompassIdentifierToken = ExpectIdentifier();

            if (lexer.PeekToken().Kind == ColonToken)
            {
                return new PortSyntax(
                    colon1Token,
                    portOrCompassIdentifierToken,
                    Expect(ColonToken),
                    ExpectIdentifier(),
                    isParsed: true);
            }

            return new PortSyntax(
                colon1Token,
                portOrCompassIdentifierToken,
                SyntaxToken.Invalid,
                SyntaxToken.Invalid,
                isParsed: true);
        }

        NodeStatementSyntax ParseNodeStatementSyntax(SyntaxToken token)
        {
            return new NodeStatementSyntax(
                ParseNodeIdentifierSyntax(token),
                ParseOptionalAttributeListSyntax(),
                ReadOptionalSemicolonToken(),
                isParsed: true);
        }

        AttributeListSyntax? ParseOptionalAttributeListSyntax()
            => lexer.PeekToken().Kind == OpenBracketToken
                ? ParseAttributeListSyntax()
                : null;

        AttributeListSyntax ParseAttributeListSyntax()
        {
            var openBracketToken = Expect(OpenBracketToken);

            List<AttributeSyntax>? attributes = null;

            while (true)
            {
                if (lexer.PeekToken().Kind == CloseBracketToken)
                {
                    return new AttributeListSyntax(
                        openBracketToken,
                        ListHelper.ListOrEmpty(attributes),
                        Expect(CloseBracketToken),
                        isParsed: true);
                }

                ListHelper.Append(ref attributes, ParseAttributeSyntax());
            }

            throw ShouldNotBeReached();
        }

        AttributeSyntax ParseAttributeSyntax()
            => new AttributeSyntax(
                ExpectIdentifier(),
                Expect(EqualsToken),
                ExpectIdentifier(),
                lexer.PeekToken().Kind switch
                {
                    CommaToken => lexer.ReadToken(),
                    SemicolonToken => lexer.ReadToken(),
                    _ => SyntaxToken.Invalid
                },
                isParsed: true);

        SyntaxToken ReadOptionalSemicolonToken()
            => lexer.PeekToken().Kind == SemicolonToken
                ? Expect(SemicolonToken)
                : SyntaxToken.Invalid;

        SyntaxToken Expect(params SyntaxKind[] kinds)
            => Expect(identifier: false, kinds);

        SyntaxToken ExpectIdentifier(params SyntaxKind[] kinds)
            => Expect(identifier: true);

        SyntaxToken ExpectIdentifierOr(params SyntaxKind[] kinds)
            => Expect(identifier: true, kinds);

        SyntaxToken Expect(bool identifier, params SyntaxKind[] kinds)
        {
            var token = lexer.ReadToken();

            if (identifier || token.IsIdentifier())
                return token;

            for (int i = 0; i < kinds.Length; i++)
            {
                if (token.Kind == kinds[i])
                    return token;
            }

            throw new Exception($"Expected one of {string.Join("|", kinds)}");
        }

        static Exception ShouldNotBeReached(string? message = null)
            => throw new Exception(message is null
                ? "Should not be reached"
                : $"Should not be reached: {message}");
    }
}
