using Refit;

namespace AiDevs2Reloaded.Api.Services.Abstractions;

public interface ICountryApi
{
    [Get("/v3.1/name/{country}?fields=population")]
    Task<dynamic> GetCountyPopulationAsync(string country, CancellationToken cancellationToken = default);
}
