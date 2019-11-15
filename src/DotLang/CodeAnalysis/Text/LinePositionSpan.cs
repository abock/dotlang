//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright (c) 2019 Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

namespace DotLang.CodeAnalysis.Text
{
    public readonly struct LinePositionSpan
    {
        public LinePosition Start { get; }
        public LinePosition End { get; }

        public LinePositionSpan(LinePosition start, LinePosition end)
        {
            Start = start;
            End = end;
        }

        public override string ToString()
            => $"({Start})-({End})";
    }
}
