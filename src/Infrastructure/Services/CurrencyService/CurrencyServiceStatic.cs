using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Services.CurrencyService
{
    public class ConversionRule
    {
        public Currency Source { get; set; }
        public Currency Target { get; set; }
        public decimal Value { get; set; }
    }

    public class CurrencyServiceStatic : ICurrencyService
    {
        private static List<ConversionRule> DEFAULT_RULES = new List<ConversionRule> {
            new ConversionRule { Source = Currency.EUR, Target = Currency.USD, Value = 0.9M },

            new ConversionRule { Source = Currency.USD, Target = Currency.EUR, Value = 1.1M },
        };

        private readonly ICollection<ConversionRule> _rules;

        public CurrencyServiceStatic(ICollection<ConversionRule> rules = null)
        {
            _rules = rules ?? DEFAULT_RULES;
        }

        public CurrencyServiceStatic(List<ConversionRule> conversionRules)
        {
        }

        /// <inheritdoc />
        public Task<decimal> Convert(decimal value, Currency source, Currency target, CancellationToken cancellationToken = default)
        {
            var conversionValue = GetConversionValue(source, target);
            var convertedValue = value * conversionValue;
            return Task.FromResult(convertedValue);
        }


        private decimal GetConversionValue(Currency source, Currency target)
        {
            var conversionRule = DEFAULT_RULES.Where(rule => rule.Source == source && rule.Target == target).FirstOrDefault();
            if (conversionRule == null)
            {
                throw new Exception("Conversion rule not found");
            }
            return conversionRule.Value;
        }
    }
}