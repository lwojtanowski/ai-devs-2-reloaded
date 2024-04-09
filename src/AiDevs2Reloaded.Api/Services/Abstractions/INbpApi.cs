using Refit;

namespace AiDevs2Reloaded.Api.Services.Abstractions;

public interface INbpApi
{
    [Get("/api/exchangerates/tables/{table}")]
    Task<dynamic> GetExchangeRatesAsync(string table, CancellationToken cancellationToken = default);

    [Get("/api/exchangerates/rates/{table}/{code}")]
    Task<dynamic> GetExchangeRateForCurrencyAsync(string table, string code, CancellationToken cancellationToken = default);
}
