﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace LabQuatro
{
    public class TestClassGenerator
    {
        private readonly string[] _sourceFiles;
        private readonly string _outputPath;
        private readonly int _maxFilesToLoad;
        private readonly int _maxConcurrentTasks;
        private readonly int _maxFilesToWrite;
        
        private readonly TaskCompletionSource<bool> _writeFilesCompletion = new TaskCompletionSource<bool>();

        public TestClassGenerator(string[] sourceFiles, string outputPath, int maxFilesToLoad, int maxConcurrentTasks,
            int maxFilesToWrite)
        {
            _sourceFiles = sourceFiles;
            _outputPath = outputPath;
            _maxFilesToLoad = maxFilesToLoad;
            _maxConcurrentTasks = maxConcurrentTasks;
            _maxFilesToWrite = maxFilesToWrite;
        }

        public async Task GenerateTestClassesAsync()
        {
            var loadFilesBlock = new TransformBlock<string, string>(
                sourceFile => LoadFileAsync(sourceFile),
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = _maxFilesToLoad });

            var generateTestsBlock = new TransformBlock<string, string>(
                sourceCode => GenerateTestClassAsync(sourceCode),
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = _maxConcurrentTasks });

            var writeFilesBlock = new ActionBlock<string>(
                testClassCode => WriteTestClassAsync(testClassCode),
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = _maxFilesToWrite });

            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            loadFilesBlock.LinkTo(generateTestsBlock, linkOptions);
            generateTestsBlock.LinkTo(writeFilesBlock, linkOptions);

            foreach (var sourceFile in _sourceFiles)
            {
                await loadFilesBlock.SendAsync(sourceFile);
            }

            loadFilesBlock.Complete();

            await writeFilesBlock.Completion;

            // Set completion for the entire process
            loadFilesBlock.Completion.ContinueWith(_ =>
            {
                if (_.IsFaulted)
                {
                    ((IDataflowBlock)generateTestsBlock).Fault(_.Exception);
                }
                else
                {
                    generateTestsBlock.Complete();
                }
            });

            await generateTestsBlock.Completion;
        }

        private async Task<string> LoadFileAsync(string sourceFile)
        {
            using (var reader = new StreamReader(sourceFile))
            {
                return await reader.ReadToEndAsync();
            }
        }

        private async Task<string> GenerateTestClassAsync(string sourceCode)
        {
            // Parse the source code into a SyntaxTree
            var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);

            // Get the root of the syntax tree
            var root = await syntaxTree.GetRootAsync();

            // Find all public methods in the syntax tree
            var publicMethods = root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .Where(m => m.Modifiers.Any(SyntaxKind.PublicKeyword));

            // Create a new SyntaxTree for the test class
            var testClassSyntax = SyntaxFactory.CompilationUnit()
                .AddUsings(
                    SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("System")),
                    SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("System.Collections.Generic")),
                    SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("System.Linq")),
                    SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("System.Text")),
                    SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("NUnit.Framework")),
                    SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("MyCode")))
                .AddMembers(
                    SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName("MyCode.Tests"))
                        .AddMembers(
                            SyntaxFactory.ClassDeclaration("MyClassTests")
                                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                                .AddAttributeLists(SyntaxFactory.AttributeList(
                                    SyntaxFactory.SingletonSeparatedList(
                                        SyntaxFactory.Attribute(SyntaxFactory.ParseName("TestFixture"))))))
                        .AddMembers(publicMethods.Select(method =>
                                (MemberDeclarationSyntax)SyntaxFactory.MethodDeclaration(
                                        SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                                        $"{method.Identifier}Test")
                                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                                    .AddAttributeLists(SyntaxFactory.AttributeList(
                                        SyntaxFactory.SingletonSeparatedList(
                                            SyntaxFactory.Attribute(SyntaxFactory.ParseName("Test")))))
                                    .WithBody(SyntaxFactory.Block(
                                        SyntaxFactory.ExpressionStatement(
                                            SyntaxFactory.InvocationExpression(
                                                    SyntaxFactory.MemberAccessExpression(
                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                        SyntaxFactory.IdentifierName("Assert"),
                                                        SyntaxFactory.IdentifierName("Fail")))
                                                .WithArgumentList(
                                                    SyntaxFactory.ArgumentList(
                                                        SyntaxFactory.SingletonSeparatedList(
                                                            SyntaxFactory.Argument(
                                                                SyntaxFactory.LiteralExpression(
                                                                    SyntaxKind.StringLiteralExpression,
                                                                    SyntaxFactory.Literal("autogenerated"))))))))))
                            .ToArray()));

            // Convert the SyntaxTree to a string
            var generatedCode = testClassSyntax.ToFullString();
            return generatedCode;
        }

        private async Task WriteTestClassAsync(string testClassCode)
        {
            var fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".cs";
            var filePath = Path.Combine(_outputPath, fileName);

            using (var writer = new StreamWriter(filePath))
            {
                await writer.WriteAsync(testClassCode);
            }
        }
    }
}