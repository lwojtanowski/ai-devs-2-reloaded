using AiDevs2Reloaded.Api.Services.Abstractions;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text.Json;

namespace AiDevs2Reloaded.Api.Plugins;

public class NbpApiPlugin
{
    private readonly INbpApi _client;
    private const string TableName = "A";

    public NbpApiPlugin(INbpApi client)
    {
        _client = client;
    }

    [KernelFunction]
    [Description("Gets the list of all exchange rates")]
    [return: Description("List of exchange rates")]
    public async Task<string> GetExchangeRates()
    {
        var exchangeRates = await _client.GetExchangeRatesAsync(TableName);
        return JsonSerializer.Serialize(exchangeRates);
    }

    [KernelFunction]
    [Description("Gets the list of all exchange rates")]
    [return: Description("List of exchange rates")]
    public async Task<string> GetExchangeRateForCurrency([Description("The Code of currency")] string currencyCode)
    {
        var exchangeRates = await _client.GetExchangeRateForCurrencyAsync(TableName, currencyCode);
        return JsonSerializer.Serialize(exchangeRates);
    }
}
