//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright (c) 2019 Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Xunit;

using DotLang.CodeAnalysis.Syntax;

namespace DotLang.Tests
{
    public class AstValidationTests
    {
        static readonly Lazy<IEnumerable<Type>> syntaxNodeTypes = new Lazy<IEnumerable<Type>>(
            () => typeof(SyntaxNode)
                .Assembly
                .GetTypes()
                .Where(type => type.IsClass
                    && typeof(SyntaxNode).IsAssignableFrom(type)));

        public static IEnumerable<object?[]> GetSyntaxNodeConstructors()
        {
            foreach (var ctor in syntaxNodeTypes.Value.SelectMany(type => type.GetConstructors()))
            {
                yield return new object?[] { ctor.DeclaringType, ctor };
            }
        }

        [Theory]
        [MemberData(nameof(GetSyntaxNodeConstructors))]
        public void VerifyNoTokenParametersInPublicCtors(Type type, ConstructorInfo ctor)
        {
            Assert.NotNull(type);
            Assert.NotNull(ctor);

            foreach (var param in ctor.GetParameters())
            {
                Assert.NotEqual(typeof(SyntaxToken), param.ParameterType);
                Assert.NotEqual(typeof(SyntaxKind), param.ParameterType);
            }
        }
    }
}
