//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright (c) 2019 Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;

using Xunit;

using DotLang.CodeAnalysis.Syntax;

namespace DotLang.Tests
{
    public class ExternalRoundTripTests
    {
        static readonly Lazy<IReadOnlyList<object[]>> graphvizTestData
            = new Lazy<IReadOnlyList<object[]>>(() =>
            {
                var list = new List<object[]>();

                var graphvizRepo = TestHelpers.GetRepositoryDirectory(
                    "external",
                    "graphviz");

                if (graphvizRepo is null)
                {
                    return list;
                }

                var skip = new HashSet<string>
                {
                    // not actually valid DOT files
                    Path.Combine(graphvizRepo, "rtest", "share", "b545.gv"),
                    Path.Combine(graphvizRepo, "rtest", "imagepath_test", "base.gv")
                };

                foreach (var graphvizFile in Directory.EnumerateFiles(
                    graphvizRepo,
                    "*.gv",
                    SearchOption.AllDirectories))
                {
                    if (skip.Contains(graphvizFile))
                    {
                        continue;
                    }

                    list.Add(new object[]
                    {
                        TestHelpers.RepositoryRootDirectory is string r
                            ? graphvizFile.Substring(r.Length + 1)
                            : graphvizFile,
                        File.ReadAllText(graphvizFile)
                    });
                }

                return list;
            });

        public static IEnumerable<object[]> GetGraphvizTestData()
            => graphvizTestData.Value;

        [Theory]
        [MemberData(nameof(GetGraphvizTestData))]
        public void AssertRoundTrip(string filePath, string sourceText)
        {
            Assert.True(
                sourceText == new Parser(sourceText).Parse().ToString(),
                filePath);
        }
    }
}
