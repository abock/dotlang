//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright (c) 2019 Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System;

namespace DotLang.CodeAnalysis.Syntax
{
    public sealed class FormattedSyntaxTextWriterSettings
    {
        public static class Defaults
        {
            public static string NewLine => Environment.NewLine;
            public static string Indent { get; } = "    ";
            public static bool InsertFinalNewLine { get; } = true;
            public static bool InsertSemicolonAfterNodeStatements { get; } = true;
            public static bool InsertSemicolonAfterSubgraphStatements { get; } = false;
        }

        public string NewLine { get; }
        public string Indent { get; }
        public bool InsertFinalNewLine { get; }
        public bool InsertSemicolonAfterNodeStatements { get; }
        public bool InsertSemicolonAfterSubgraphStatements { get; }

        internal char LastNewLineChar => NewLine[NewLine.Length - 1];

        public FormattedSyntaxTextWriterSettings(
            string? newLine = null,
            string? indent = null,
            bool? insertFinalNewLine = null,
            bool? insertSemicolonAfterNodeStatements = null,
            bool? insertSemicolonAfterSubgraphStatements = null)
        {
            NewLine = newLine
                ?? Defaults.NewLine;
            Indent = indent
                ?? Defaults.Indent;
            InsertFinalNewLine = insertFinalNewLine
                ?? Defaults.InsertFinalNewLine;
            InsertSemicolonAfterNodeStatements = insertSemicolonAfterNodeStatements
                ?? Defaults.InsertSemicolonAfterNodeStatements;
            InsertSemicolonAfterSubgraphStatements = insertSemicolonAfterSubgraphStatements
                ?? Defaults.InsertSemicolonAfterSubgraphStatements;

            if (NewLine.Length < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(newLine),
                    "must contain at least one character");
            }
        }
    }
}
