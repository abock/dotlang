//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright (c) 2019 Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Text;

namespace DotLang.Tests
{
    static class TestHelpers
    {
        /// <summary>
        /// Returns the absolute path of the root git repository if the loaded
        /// test assembly is anywhere in the repository's directory structure.
        /// </summary>
        public static string? RepositoryRootDirectory { get; }

        static TestHelpers()
        {
            var path = Path.GetDirectoryName(typeof(TestHelpers).Assembly.Location);
            while (path is string && Directory.Exists(path))
            {
                if (Directory.Exists(Path.Combine(path, ".git")))
                {
                    RepositoryRootDirectory = Path.GetFullPath(path);
                    break;
                }

                path = Path.GetDirectoryName(path);
            }
        }

        /// <summary>
        /// Returns the absolute path of the provided relative directory
        /// path within the git repository if the loaded test assembly is
        /// anywhere in the repository's directory structure.
        /// </summary>
        public static string? GetRepositoryDirectory(params string[] pathComponents)
        {
            var repoRoot = RepositoryRootDirectory;
            if (repoRoot is null)
            {
                return null;
            }

            return Path.Combine(
                repoRoot,
                Path.Combine(pathComponents));
        }

        public static string? TrimCommonLeadingWhiteSpace(
            string? str,
            bool trimEmptyFirstLine = true,
            bool trimEmptyLastLine = true)
        {
            if (str is null)
            {
                return null;
            }

            string? leading = null;
            var untrimmedLines = str.Split('\n');
            var trimmedLines = new StringBuilder(str.Length);
            int writtenLines = 0;

            for (int i = 0; i < untrimmedLines.Length; i++)
            {
                var line = untrimmedLines[i];

                if (trimEmptyFirstLine
                    && i == 0
                    && string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                else if (trimEmptyLastLine
                    && i == untrimmedLines.Length - 1
                    && string.IsNullOrWhiteSpace(line))
                {
                    break;
                }

                if (leading is null)
                {
                    for (int j = 0; j < line.Length; j++)
                    {
                        if (!char.IsWhiteSpace(line[j]))
                        {
                            if (j > 0)
                            {
                                leading = line.Substring(0, j - 1);
                            }
                            break;
                        }
                    }
                }

                if (leading is null)
                {
                    return str;
                }
                else if (line.StartsWith(leading, StringComparison.Ordinal))
                {
                    if (writtenLines++ > 0)
                    {
                        trimmedLines.Append('\n');
                    }

                    trimmedLines.Append(line.Substring(leading.Length + 1));
                }
                else
                {
                    return str;
                }
            }

            return trimmedLines.ToString();
        }
    }
}
