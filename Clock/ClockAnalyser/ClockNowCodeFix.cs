﻿using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ClockAnalyser
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ClockNowCodeFix)), Shared]
    public class ClockNowCodeFix : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(ClockNowAnalyser.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var expressionSyntax = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MemberAccessExpressionSyntax>().First();

            var codeAction = CodeAction.Create(ClockNowAnalyser.Title, cancellationToken => ChangeToDateTimeProvider(context.Document, expressionSyntax, cancellationToken), ClockNowAnalyser.Title);
            context.RegisterCodeFix(codeAction, diagnostic);
        }

        private static async Task<Document> ChangeToDateTimeProvider(Document document, SyntaxNode syntaxNode, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(syntaxNode, SyntaxFactory.ParseExpression($"Clock.UtcNow"));
            return document.WithSyntaxRoot(newRoot);
        }
    }
}