using FluentValidation;
using Squrl.App.Features.PurchaseOrderDetails.DTOs;

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
    
    extension<T>(IRuleBuilder<T, List<CreatePoDetailDto>?> ruleBuilder)
    {
        public IRuleBuilderOptions<T, List<CreatePoDetailDto>?> BeSequential()
        {
            return ruleBuilder.Must(poDetails =>
            {
                if (poDetails is null || poDetails.Count == 0)
                {
                    return false;
                }

                if (poDetails.Any(p => !p.LineSequence.HasValue))
                {
                    return false;
                }

                int[] sequences = poDetails
                    .Select(p => p.LineSequence ?? 0)
                    .OrderBy(p => p)
                    .ToArray();

                return sequences
                    .Select((value, index) => value == index + 1)
                    .All(x => x);
            });
        }
    }
    
    extension<T>(IRuleBuilder<T, List<UpdatePoDetailDto>?> ruleBuilder)
    {
        public IRuleBuilderOptions<T, List<UpdatePoDetailDto>?> BeSequential()
        {
            return ruleBuilder.Must(poDetails =>
            {
                if (poDetails is null || poDetails.Count == 0)
                {
                    return false;
                }

                if (poDetails.Any(p => !p.LineSequence.HasValue))
                {
                    return false;
                }

                int[] sequences = poDetails
                    .Select(p => p.LineSequence ?? 0)
                    .OrderBy(p => p)
                    .ToArray();

                return sequences
                    .Select((value, index) => value == index + 1)
                    .All(x => x);
            });
        }
    }
}