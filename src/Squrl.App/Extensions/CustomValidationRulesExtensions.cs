using FluentValidation;
using Squrl.App.Features.PurchaseOrderDetails.DTOs;
using Squrl.App.Features.SalesOrderDetails.DTOs;

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
        
        public IRuleBuilderOptions<T, string?> BeAlphanumeric()
        {
            return ruleBuilder.Must(x => x is null || x.All(c =>
                c is >= 'A' and <= 'Z'
                    or >= 'a' and <= 'z'
                    or >= '0' and <= '9'));
        }
    }
    
    extension<T>(IRuleBuilder<T, List<CreateSoDetailDto>?> ruleBuilder)
    {
        public IRuleBuilderOptions<T, List<CreateSoDetailDto>?> BeSequential()
        {
            return ruleBuilder.Must(soDetails =>
            {
                if (soDetails is null || soDetails.Count == 0)
                {
                    return false;
                }

                if (soDetails.Any(s => !s.LineSequence.HasValue))
                {
                    return false;
                }

                int[] sequences = soDetails
                    .Select(s => s.LineSequence ?? 0)
                    .OrderBy(s => s)
                    .ToArray();

                return sequences
                    .Select((value, index) => value == index + 1)
                    .All(x => x);
            });
        }
    }
    
    extension<T>(IRuleBuilder<T, List<UpdateSoDetailDto>?> ruleBuilder)
    {
        public IRuleBuilderOptions<T, List<UpdateSoDetailDto>?> BeSequential()
        {
            return ruleBuilder.Must(soDetails =>
            {
                if (soDetails is null || soDetails.Count == 0)
                {
                    return false;
                }

                if (soDetails.Any(s => !s.LineSequence.HasValue))
                {
                    return false;
                }

                int[] sequences = soDetails
                    .Select(s => s.LineSequence ?? 0)
                    .OrderBy(s => s)
                    .ToArray();

                return sequences
                    .Select((value, index) => value == index + 1)
                    .All(x => x);
            });
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