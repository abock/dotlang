//
// Author:
//   Aaron Bockover <aaron@abock.dev>
//
// Copyright (c) 2019 Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using DotLang.CodeAnalysis.Syntax;
using DotLang.CodeAnalysis.Text;

namespace DotLang
{
    static class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.Error.WriteLine("usage: dotdump [GRAPHVIZ_DOT_FILE | - <STDIN>] | -ast [AST_OUTPUT_DOT_FILE]");
            }
            else if (args[0] == "-ast")
            {
                var graph = BuildAstGraph();

                if (args.Length > 1)
                {
                    File.WriteAllText(args[1], graph.ToString());
                }
                else
                {
                    Console.WriteLine(graph);
                }
            }
            else
            {
                var dumper = new TreeDumper();
                var parser = new Parser(args[0] == "-"
                    ? SourceText.From(Console.In)
                    : SourceText.From(File.ReadAllText(args[0])));
                parser.Parse().Accept(dumper);
            }
        }

        static DigraphSyntax BuildAstGraph()
        {
            var types = typeof(SyntaxNode)
                .Assembly
                .GetTypes()
                .Where(type => type.IsClass
                    && typeof(SyntaxNode).IsAssignableFrom(type))
                .OrderBy(type => Depth(type))
                .ThenBy(type => type.BaseType?.Name)
                .ThenBy(type => type.Name);

            return new DigraphSyntax(
                new StatementSyntax[]
                {
                    ("rankdir", "LR"),
                    ("bgcolor", "transparent"),
                    new AttributeStatementSyntax(
                        AttributeKeyword.Node,
                        ("fontname", "sans-serif"),
                        ("color", "#102538"),
                        ("fontcolor", "#337ab7"),
                        ("shape", "record")),
                    new AttributeStatementSyntax(
                        AttributeKeyword.Edge,
                        ("color", "#3B5266")),
                }.Concat(types
                    .Select(type => new NodeStatementSyntax(
                        type.Name,
                        ("target", "_parent"),
                        ("href", $"/api/{type.FullName}.html"))))
                .Concat(types
                    .Where(type => type.BaseType != typeof(object))
                    .Select(type => new EdgeStatementSyntax(
                        type.Name,
                        EdgeKind.Directed,
                        type.BaseType?.Name ?? "(unknown)"))));

            static int Depth(Type? type)
            {
                int depth = 0;
                while (type is Type)
                {
                    depth++;
                    type = type.BaseType;
                }
                return depth;
            }
        }

        static List<(Type Type, int Depth)> TopologicalSort(this IEnumerable<Type> types)
        {
            var visited = new HashSet<Type>();
            var sorted = new List<(Type, int)>();

            void Visit(Type type, int depth)
            {
                if (visited.Add(type) && type.BaseType is Type baseType)
                {
                    Visit(baseType, depth + 1);
                    sorted.Add((type, depth));
                }
            }

            foreach (var type in types)
            {
                Visit(type, 0);
            }

            return sorted;
        }

        sealed class TreeDumper : SyntaxNodeDumper
        {
            readonly TextWriter writer = Console.Out;

            readonly Stack<string> indents = new Stack<string>();

            protected override bool DumpNode(SyntaxNode node, VisitKind visitKind)
            {
                string GetIndent(int depth, bool lastChild)
                    => new string(' ', depth * 2);

                if (visitKind == VisitKind.Enter)
                {
                    writer.Write(GetIndent(NodeStack.Count - 1, false));
                    writer.Write(node.GetType().Name);
                    writer.WriteLine();
                }
                else if (visitKind == VisitKind.Leave)
                {
                    var tokens = NodeStack[0].Tokens;
                    for (int i = 0; i < tokens.Count; i++)
                    {
                        var token = tokens[i];
                        writer.Write(GetIndent(NodeStack.Count, i == tokens.Count - 1));
                        writer.Write(token.Kind);

                        switch (token.Kind)
                        {
                            case SyntaxKind.WhiteSpaceTriviaToken:
                                writer.WriteLine();
                                break;
                            default:
                                writer.Write(" → ");
                                writer.WriteLine(token.SourceText);
                                break;
                        }
                    }
                }

                return true;
            }
        }
    }
}
