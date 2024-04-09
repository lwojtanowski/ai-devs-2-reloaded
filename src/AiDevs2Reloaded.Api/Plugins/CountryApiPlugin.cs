using AiDevs2Reloaded.Api.Services.Abstractions;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text.Json;

namespace AiDevs2Reloaded.Api.Plugins;

public class CountryApiPlugin
{
    private readonly ICountryApi _client;

    public CountryApiPlugin(ICountryApi client)
    {
        _client = client;
    }

    [KernelFunction]
    [Description("Get information about population of the country")]
    [return: Description("Information about population of the country")]
    public async Task<string> GetExchangeRates(string countryName)
    {
        var response = await _client.GetCountyPopulationAsync(countryName);
        List<CountryPopulationResponse> results = JsonSerializer.Deserialize<List<CountryPopulationResponse>>(response);
        return $"Population of {countryName} is {results.Select(x => x.population).FirstOrDefault()}";
    }

    private sealed record CountryPopulationResponse(long population);
}