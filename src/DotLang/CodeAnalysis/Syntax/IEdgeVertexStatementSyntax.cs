//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright (c) 2019 Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

namespace DotLang.CodeAnalysis.Syntax
{
    public interface IEdgeVertexStatementSyntax
    {
        void Accept(SyntaxVisitor visitor);
    }
}
