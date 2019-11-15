# DotLang: DOT Language Library for  .NET

DotLang provides a full fidelity lexer, recursive descent parser, and
abstract syntax tree for the [Graphviz][graphviz] [DOT Language][dot-lang],
written in modern C# 8.0. It is available as a `netstandard2.0` library.

## Goals

The primary goal of this project is to ultimately provide a language
service for Visual Studio that provides rich syntactic and semantic
analysis of  Graphviz DOT files: syntax highlighting, semantic
completion, and Quick Info tooltips.

To support this goal, the lexer and parser are written with inspiration
from [Roslyn][roslyn]: nothing is ever discarded, and the produced AST
can be run through a `SyntaxVisitor` to produce output DOT source code
that is identical to its input.

### Anti-Goals

Notably, DotLang does _not_ aspire to actually render [Graphviz][graphviz]
graphs. There are plenty of tools for this already.

## TODO

- Actual diagnostics
  - Currently the parser just throws/bails
- Break dependency on `Microsoft.CodeAnalysis.Common`:
  - `SourceText`
  - `TextSpan`
- Language service
  - Would be awesome to leverage XML/HTML projection buffers for `XmlLiteralToken`
- Documentation

[graphviz]: https://graphviz.gitlab.io/
[dot-lang]: https://graphviz.gitlab.io/_pages/doc/info/lang.html
[roslyn]: https://github.com/dotnet/roslyn