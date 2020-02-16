using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Services.CurrencyService
{
    public class CurrencyServiceExternal : ICurrencyService
    {
        /// <inheritdoc />
        public Task<decimal> Convert(decimal value, Currency source, Currency target, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(value);
        }
    }
}