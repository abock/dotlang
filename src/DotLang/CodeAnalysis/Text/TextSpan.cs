//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright (c) 2019 Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

namespace DotLang.CodeAnalysis.Text
{
    public readonly struct TextSpan
    {
        public static TextSpan FromBounds(int start, int end)
            => new TextSpan(start, end - start);

        public int Start { get; }
        public int End => Start + Length;
        public int Length { get; }

        public TextSpan(int start, int length)
        {
            Start = start;
            Length = length;
        }

        public override string ToString()
            => $"[{Start}..{End})";
    }
}
