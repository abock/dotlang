//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright (c) 2019 Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

using DotLang.CodeAnalysis.Text;

namespace DotLang.CodeAnalysis.Syntax
{
    /// <summary>
    /// Represents a tokenized portion of source text that is identified with
    /// a <see cref="SyntaxKind"/> and may contain a parsed value in
    /// <see cref="SyntaxToken.StringValue"/>. Tokens also carry leading trivia
    /// which are usually uninteresting tokens (white space, comments). A token
    /// may be fully round-tripped through its <see cref="SyntaxToken.SourceText"/>
    /// property.
    /// </summary>
    public sealed class SyntaxToken : SyntaxNode
    {
        public static SyntaxToken Invalid { get; } = new SyntaxToken(
            SyntaxKind.None,
            default,
            null,
            null,
            Array.Empty<SyntaxToken>());

        /// <summary>
        /// The syntax kind of this token.
        /// </summary>
        public SyntaxKind Kind { get; }

        /// <summary>
        /// The location, including start and end points, of this token in its
        /// original source text.
        /// </summary>
        public SourceLocation Location { get; }

        /// <summary>
        /// The original source text for this token (based on <see cref="Location"/>).
        /// </summary>
        public string? SourceText { get; }

        /// <summary>
        /// A semantic string representation of this token's value.
        /// </summary>
        public string? StringValue { get; }

        /// <summary>
        /// Any potential leading trivia tokens that precede this token such
        /// as white space or comments.
        /// </summary>
        public IReadOnlyList<SyntaxToken> LeadingTrivia { get; }

        internal SyntaxToken(
            SyntaxKind kind,
            SourceLocation location,
            string? sourceText,
            string? stringValue,
            IReadOnlyList<SyntaxToken> leadingTrivia)
            : base(isParsed: true)
        {
            Kind = kind;
            Location = location;
            SourceText = sourceText;
            StringValue = stringValue;
            LeadingTrivia = leadingTrivia;
        }

        internal SyntaxToken(
            SyntaxKind kind,
            string? stringValue = null)
            : base(isParsed: false)
        {
            Kind = kind;
            StringValue = stringValue;

            Location = default;
            SourceText = null;
            LeadingTrivia = Array.Empty<SyntaxToken>();
        }

        public override void Accept(SyntaxVisitor visitor)
        {
            if (Kind != SyntaxKind.None)
            {
                foreach (var trivia in LeadingTrivia)
                {
                    trivia.Accept(visitor);
                }

                visitor.VisitSyntaxToken(this);
            }
        }
    }
}
