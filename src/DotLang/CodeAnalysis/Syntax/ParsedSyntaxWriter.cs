//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright (c) 2019 Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;

namespace DotLang.CodeAnalysis.Syntax
{
    /// <summary>
    /// A <see cref="SyntaxVisitor"/> that produces source text from a syntax tree
    /// based only on <see cref="SyntaxToken.SourceText"/>, a scenario only valid
    /// for syntax trees constructed through <see cref="Parser"/>.
    /// </summary>
    public sealed class ParsedSyntaxWriter : SyntaxVisitor
    {
        readonly TextWriter writer;

        public ParsedSyntaxWriter(TextWriter writer)
            => this.writer = writer
                ?? throw new ArgumentNullException(nameof(writer));

        public override void VisitSyntaxToken(SyntaxToken token)
        {
            if (token.SourceText is string)
            {
                writer.Write(token.SourceText);
                return;
            }
        }

        public override bool VisitSyntaxNode(
            SyntaxNode node,
            VisitKind visitKind)
        {
            if (!node.IsParsed)
            {
                throw new InvalidOperationException(
                    $"Node was not produced via {nameof(Parser)}; " +
                    $"use {nameof(FormattedSyntaxWriter)} instead");
            }

            return true;
        }
    }
}
