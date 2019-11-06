using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Text;

namespace Meziantou.Analyzer.Rules
{
    [ExportCodeFixProvider(LanguageNames.CSharp), Shared]
    public sealed class AbstractTypesShouldNotHaveConstructorsFixer : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(RuleIdentifiers.AbstractTypesShouldNotHaveConstructors);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var codeAction = CodeAction.Create("Abstract types should not have public or internal constructors",
                cancellationToken => ChangeToDateTimeProvider(context.Document,
                    diagnosticSpan,
                    cancellationToken),
                "Abstract types should not have public or internal constructors");
            context.RegisterCodeFix(codeAction, diagnostic);
        }

        private static async Task<Document> ChangeToDateTimeProvider(Document document, TextSpan span, CancellationToken cancellationToken)
        {
            var text = await document.GetTextAsync(cancellationToken);
            var repl = "Clock.UtcNow";
            var newtext = text.Replace(span, repl);
            return document.WithText(newtext);
        }
    }
}
