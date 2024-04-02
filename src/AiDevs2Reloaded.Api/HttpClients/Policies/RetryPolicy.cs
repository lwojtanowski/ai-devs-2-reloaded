using Polly;
using Polly.Extensions.Http;
using System.Net;

namespace AiDevs2Reloaded.Api.HttpClients.Policies;

public static partial class  Policy
{
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retryCount = 3)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == HttpStatusCode.Forbidden)
            .WaitAndRetryAsync(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }
}
