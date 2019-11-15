//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright (c) 2019 Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;

namespace DotLang.CodeAnalysis.Syntax
{
    public abstract class SyntaxNode
    {
        public bool IsParsed { get; }

        private protected SyntaxNode(bool isParsed)
            => IsParsed = isParsed;

        public abstract void Accept(SyntaxVisitor visitor);

        public override string ToString()
        {
            using var writer = new StringWriter();
            Write(writer);
            return writer.ToString();
        }

        public void Write(TextWriter writer)
            => Accept(IsParsed
                ? (SyntaxVisitor)new ParsedSyntaxWriter(writer)
                : (SyntaxVisitor)new FormattedSyntaxWriter(writer));

        private protected static List<T> ToList<T>(IEnumerable<T>? items)
            => items is null
                ? new List<T>()
                : new List<T>(items);
    }
}
