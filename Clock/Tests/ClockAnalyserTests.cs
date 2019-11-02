using System;
using ClockAnalyser;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Tests.TestHelpers;
using Xunit;

namespace Tests
{
    public class ClockAnalyserTests : CodeFixVerifier
    {
        private const string SourceCodeWithFix = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            public TypeName()
            {
                var now = Clock.UtcNow;
            }
        }
    }";
        
        
        private string GetSourceCodeWithIssue(TimeNowTypes timeNowTypes)
        {
            var code = @"
    using System;

    namespace ConsoleApplication1
    {{
        class TypeName
        {{
            public TypeName()
            {{
                var now = {0};
            }}
        }}
    }}";
            string issue = GetIssueValue(timeNowTypes);
            
            return string.Format(code, issue);
        }

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

        [Theory]
        [InlineData(TimeNowTypes.DateTimeNow)]
        [InlineData(TimeNowTypes.DateTimeOffsetNow)]
        [InlineData(TimeNowTypes.DateTimeUtcNow)]
        [InlineData(TimeNowTypes.DateTimeOffsetUtcNow)]
        private void IdentifyDateTimeNowProblem(TimeNowTypes errorType)
        {
            var expected = new DiagnosticResult
            {
                Id = "ClockNowAnalyser",
                Message = "Use Clock.UtcNow instead of DateTime",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {new DiagnosticResultLocation("Test0.cs", 10, 27)}
            };

            VerifyCSharpDiagnostic(GetSourceCodeWithIssue(errorType), expected);
        }

        [Theory]
        [InlineData(TimeNowTypes.DateTimeNow)]
        [InlineData(TimeNowTypes.DateTimeOffsetNow)]
        [InlineData(TimeNowTypes.DateTimeUtcNow)]
        [InlineData(TimeNowTypes.DateTimeOffsetUtcNow)]
        private void ApplySuggestedFix(TimeNowTypes errorType)
        {
            var expected = new DiagnosticResult
            {
                Id = "ClockNowAnalyser",
                Message = "Use Clock.UtcNow instead of DateTime",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {new DiagnosticResultLocation("Test0.cs", 10, 27)}
            };

            VerifyCSharpDiagnostic(GetSourceCodeWithIssue(errorType), expected);
            VerifyCSharpFix(GetSourceCodeWithIssue(errorType), SourceCodeWithFix, null, true);
        }
        
        
        
        private enum TimeNowTypes
        {
            DateTimeNow = 0,
            DateTimeUtcNow = 1,
            DateTimeOffsetNow = 2,
            DateTimeOffsetUtcNow = 3
        }
        
        
        private string GetIssueValue(TimeNowTypes timeNowTypes)
        {
            switch (timeNowTypes)
            {
                case TimeNowTypes.DateTimeNow:
                    return "DateTime.Now";
                case TimeNowTypes.DateTimeUtcNow:
                    return "DateTime.UtcNow";
                case TimeNowTypes.DateTimeOffsetNow:
                    return "DateTimeOffset.Now";
                case TimeNowTypes.DateTimeOffsetUtcNow:
                    return "DateTimeOffset.UtcNow";
                default:
                    throw new ArgumentOutOfRangeException(nameof(timeNowTypes), timeNowTypes, null);
            }
        }
    }
}