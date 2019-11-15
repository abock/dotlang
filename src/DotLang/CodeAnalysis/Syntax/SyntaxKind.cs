//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright (c) 2019 Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

namespace DotLang.CodeAnalysis.Syntax
{
    /// <summary>
    /// Identifies a kind for a <see cref="SyntaxToken"/>, the basis for which
    /// <see cref="Parser"/> will construct the syntax tree.
    /// </summary>
    public enum SyntaxKind : byte
    {
        /// <summary>
        /// No syntax has been identified. Implies an invalid <see cref="SyntaxToken"/>.
        /// </summary>
        None,

        /// <summary>
        /// The <c>=</c> literal character.
        /// </summary>
        EqualsToken,

        /// <summary>
        /// The <c>{</c> literal character.
        /// </summary>
        OpenBraceToken,

        /// <summary>
        /// The <c>}</c> literal character.
        /// </summary>
        CloseBraceToken,

        /// <summary>
        /// The <c>[</c> literal character.
        /// </summary>
        OpenBracketToken,

        /// <summary>
        /// The <c>]</c> literal character.
        /// </summary>
        CloseBracketToken,

        /// <summary>
        /// The <c>:</c> literal character.
        /// </summary>
        ColonToken,

        /// <summary>
        /// The <c>;</c> literal character.
        /// </summary>
        SemicolonToken,

        /// <summary>
        /// The <c>,</c> literal character.
        /// </summary>
        CommaToken,

        /// <summary>
        /// Syntax for a numeral literal <c>[-]?(.[0-9]+ | [0-9]+(.[0-9]*)?)</c>.
        /// </summary>
        NumeralLiteralToken,

        /// <summary>
        /// Syntax for a double-quoted string literal <c>&quot;...&quot;</c>, possibly containing
        /// escaped quotes (<c>\\&quot;</c>). In quoted strings in DOT, the only escaped character
        /// is double-quote (<c>&quot;</c>). That is, in quoted strings, the dyad <c>\\&quot;</c>
        /// is converted to <c>&quot;</c> and all other characters are left unchanged. In particular,
        /// <c>\\\\</c> remains <c>\\\\</c>. Layout engines may apply additional escape sequences.
        /// </summary>
        StringLiteralToken,

        /// <summary>
        /// Syntax for an HTML/XML string literal <c>&lt;...&gt;</c>. Angle brackets must occur in
        /// matched pairs, and newlines and other formatting whitespace characters are allowed.
        /// </summary>
        XmlLiteralToken,

        /// <summary>
        /// Syntax for an identifier, which is any string of alphabetic characters, underscores
        /// (<c>'_'</c>) or digits (<c>[0-9]</c>), not beginning with a digit. An identifier is
        /// just a string; the lack of quote characters is just for simplicity: there is no semantic
        /// difference between <c>abc_2</c> and <c>&quot;abc_2&quot;</c>. To use a keyword as an
        /// identifier, it must be quoted.
        /// </summary>
        IdentifierToken,

        /// <summary>
        /// Syntax for the <c>-&gt;</c> directed edge token.
        /// </summary>
        DirectedEdgeToken,

        /// <summary>
        /// Syntax for the <c>--</c> undirected edge token.
        /// </summary>
        UndirectedEdgeToken,

        /// <summary>
        /// Indicates the last token in the source text indicating the end. No more tokens
        /// will follow. Like all non-trivia tokens, this token may have leading trivia
        /// (that is, trivia at the end of the file, such as a final new line).
        /// </summary>
        EndOfFileToken,

        /// <summary>
        /// Syntax for the <c>strict</c> keyword.
        /// </summary>
        StrictKeyword,

        /// <summary>
        /// Syntax for the <c>graph</c> keyword.
        /// </summary>
        GraphKeyword,

        /// <summary>
        /// Syntax for the <c>digraph</c> keyword.
        /// </summary>
        DigraphKeyword,

        /// <summary>
        /// Syntax for the <c>subgraph</c> keyword.
        /// </summary>
        SubgraphKeyword,

        /// <summary>
        /// Syntax for the <c>node</c> keyword.
        /// </summary>
        NodeKeyword,

        /// <summary>
        /// Syntax for the <c>edge</c> keyword.
        /// </summary>
        EdgeKeyword,

        /// <summary>
        /// Trivia syntax that may appear in <see cref="SyntaxToken.LeadingTrivia"/>
        /// representing a a run of white space characters, including new lines and
        /// carriage returns.
        /// </summary>
        WhiteSpaceTriviaToken,

        /// <summary>
        /// Trivia syntax that may appear in <see cref="SyntaxToken.LeadingTrivia"/>
        /// representing C-style (<c>//</c>) single-line comments.
        /// </summary>
        SingleLineCommentTriviaToken,

        /// <summary>
        /// Trivia syntax that may appear in <see cref="SyntaxToken.LeadingTrivia"/>
        /// representing C++-style (<c>/*&#160;*/</c>) multi-line comments.
        /// </summary>
        MultiLineCommentTriviaToken,

        /// <summary>
        /// Syntax representing a C preprocessor directive line (<c>#line 34</c>).
        /// </summary>
        PreprocessorDirective
    }
}
