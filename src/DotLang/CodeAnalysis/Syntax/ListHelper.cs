//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright (c) 2019 Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace DotLang.CodeAnalysis.Syntax
{
    static class ListHelper
    {
        public static void Append<T>(
            ref List<T>? list,
            T item,
            int initialCapacity = 4)
        {
            if (list is null)
            {
                list = new List<T>(initialCapacity);
            }

            list.Add(item);
        }

        public static IReadOnlyList<T> ListOrEmpty<T>(List<T>? list)
            => list ?? (IReadOnlyList<T>)Array.Empty<T>();
    }
}
