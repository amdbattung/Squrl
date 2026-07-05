using FluentValidation;

namespace Squrl.App.Extensions;

public static class CustomValidationRulesExtensions
{
    extension<T>(IRuleBuilder<T, string?> ruleBuilder)
    {
        public IRuleBuilderOptions<T, string?> BeNullOrNonWhitespace()
        {
            return ruleBuilder.Must(x => x == null || !string.IsNullOrWhiteSpace(x));
        }

        public IRuleBuilderOptions<T, string?> BeAscii()
        {
            return ruleBuilder.Must(x => x is null || x.All(c => c >= 32 && c <= 126));
        }
    }
}