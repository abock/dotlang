//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright (c) 2019 Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

namespace DotLang.CodeAnalysis.Syntax
{
    public abstract class StatementSyntax : SyntaxNode
    {
        public SyntaxToken TerminatingSemicolonToken { get; }

        private protected StatementSyntax(
            SyntaxToken terminatingSemicolonToken,
            bool isParsed)
            : base(isParsed)
        {
            TerminatingSemicolonToken = terminatingSemicolonToken;
        }

        public static implicit operator StatementSyntax((string Name, string Value) nameValue)
            => new NameValueStatementSyntax(nameValue);
    }
}
