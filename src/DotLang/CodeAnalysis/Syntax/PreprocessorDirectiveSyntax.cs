//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright (c) 2019 Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

namespace DotLang.CodeAnalysis.Syntax
{
    public sealed class PreprocessorDirectiveSyntax : SyntaxNode
    {
        public SyntaxToken Directive { get; }

        internal PreprocessorDirectiveSyntax(SyntaxToken directive, bool isParsed)
            : base(isParsed)
            => Directive = directive;

        public override void Accept(SyntaxVisitor visitor)
        {
            if (visitor.VisitPreprocessorDirectiveSyntax(this, VisitKind.Enter))
            {
                Directive.Accept(visitor);

                visitor.VisitPreprocessorDirectiveSyntax(this, VisitKind.Leave);
            }
        }
    }
}
