using DateTimeProviderAnalyser.DateTimeNow;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Tests.TestHelpers;
using Xunit;

namespace Tests
{
    public class ClockAnalyserTests : CodeFixVerifier
    {
        private const string SourceCodeWithIssue = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            public TypeName()
            {
                var now = DateTimeOffset.Now;
            }
        }
    }";

        private const string SourceCodeWithFix = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            public TypeName()
            {
                var now = DateTimeProvider.Now;
            }
        }
    }";

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new ClockNowCodeFix();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new ClockNowAnalyser();
        }

        [Fact]
        public void ExpectNoDiagnosticResults()
        {
            const string source = @"";
            VerifyCSharpDiagnostic(source);
        }

        [Fact]
        public void IdentifySuggestedFix()
        {
            var expected = new DiagnosticResult
            {
                Id = "ClockNowAnalyser",
                Message = "Use Clock.UtcNow instead of DateTime",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {new DiagnosticResultLocation("Test0.cs", 10, 27)}
            };

            VerifyCSharpDiagnostic(SourceCodeWithIssue, expected);
        }

        [Fact]
        public void ApplySuggestedFix()
        {
            var expected = new DiagnosticResult
            {
                Id = "ClockNowAnalyser",
                Message = "Use Clock.UtcNow instead of DateTime",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {new DiagnosticResultLocation("Test0.cs", 10, 27)}
            };

            VerifyCSharpDiagnostic(SourceCodeWithIssue, expected);
            VerifyCSharpFix(SourceCodeWithIssue, SourceCodeWithFix, null, true);
        }
    }
}