//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright (c) 2019 Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

namespace DotLang.CodeAnalysis.Text
{
    public readonly struct SourceLocation
    {
        public TextSpan SourceSpan { get; }
        public LinePositionSpan LinePositionSpan { get; }

        public SourceLocation(
            TextSpan sourceSpan,
            LinePositionSpan linePositionSpan)
        {
            SourceSpan = sourceSpan;
            LinePositionSpan = linePositionSpan;
        }
    }
}
