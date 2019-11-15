//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright (c) 2019 Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DotLang.CodeAnalysis.Text
{
    public sealed class SourceText
    {
        public static SourceText From(string? value)
            => new SourceText(value);

        public static SourceText From(TextReader reader)
            => new SourceText(reader.ReadToEnd());

        public static async Task<SourceText> FromAsync(
            TextReader reader, CancellationToken cancellationToken = default)
            => new SourceText(await reader.ReadToEndAsync().ConfigureAwait(false));

        readonly string value;

        SourceText(string? value)
        {
            this.value = value ?? string.Empty;
        }

        public int Length => value.Length;
        public char this[int index] => value[index];

        public string ToString(TextSpan span)
        {
            if (span.Start == 0 && span.Length == value.Length)
            {
                return value;
            }

            return value.Substring(span.Start, span.Length);
        }

        public override string ToString()
            => ToString(new TextSpan(0, Length));
    }
}
