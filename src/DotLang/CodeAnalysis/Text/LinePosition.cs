//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright (c) 2019 Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

namespace DotLang.CodeAnalysis.Text
{
    public readonly struct LinePosition
    {
        public int Line { get; }
        public int Character { get; }

        public LinePosition(int line, int character)
        {
            Line = line;
            Character = character;
        }

        public override string ToString()
            => $"{Line},{Character}";
    }
}
