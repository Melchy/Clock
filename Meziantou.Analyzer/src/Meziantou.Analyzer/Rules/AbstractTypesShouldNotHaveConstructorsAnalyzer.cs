using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Meziantou.Analyzer.Rules
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class AbstractTypesShouldNotHaveConstructorsAnalyzer : DiagnosticAnalyzer
    {
        private static readonly DiagnosticDescriptor s_rule = new DiagnosticDescriptor(
            RuleIdentifiers.AbstractTypesShouldNotHaveConstructors,
            title: "Abstract types should not have public or internal constructors",
            messageFormat: "Abstract types should not have public or internal constructors",
            RuleCategories.Design,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "",
            helpLinkUri: RuleIdentifiers.GetHelpUri(RuleIdentifiers.AbstractTypesShouldNotHaveConstructors));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(s_rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.SimpleMemberAccessExpression);
        }

        private static void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            // The analyzer will run on every keystroke in the editor, so we are performing the quickest tests first
            var member = context.Node as MemberAccessExpressionSyntax;
            var identifier = member?.Expression as IdentifierNameSyntax;

            if (identifier == null)
                return;

            if (identifier.Identifier.Text != nameof(DateTime)
                && identifier.Identifier.Text != nameof(DateTimeOffset))
                return;

            var identifierSymbol = ModelExtensions.GetSymbolInfo(context.SemanticModel, identifier).Symbol as INamedTypeSymbol;
            if (identifierSymbol?.ContainingNamespace.ToString() != nameof(System))
                return;

            var accessor = member.Name.ToString();
            if (accessor != nameof(DateTime.Now) &&
                accessor != nameof(DateTime.UtcNow))
                return;

            var rule = s_rule;
            var diagnostic = Diagnostic.Create(rule, member.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }
}
