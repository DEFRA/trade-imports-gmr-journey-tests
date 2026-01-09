using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace TradeImportsGmr.JourneyTests.Clients.GmrFinder;

public sealed class GmrFinderMessageClient(HttpClient httpClient) : IDisposable
{
    public static GmrFinderMessageClient Create()
    {
        var options = GmrFinderMessageClientOptions.FromConfiguration();

        var httpClient = new HttpClient { BaseAddress = options.BaseUri, Timeout = options.Timeout };
        if (!string.IsNullOrWhiteSpace(options.ApiKey))
        {
            httpClient.DefaultRequestHeaders.Add("X-Api-Key", options.ApiKey);
        }
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Basic",
            Convert.ToBase64String(Encoding.UTF8.GetBytes(options.BasicAuth))
        );

        return new GmrFinderMessageClient(httpClient);
    }

    public async Task<HttpResponseMessage> SendDataEventAsync(
        string resourceType,
        string body,
        CancellationToken cancellationToken = default
    )
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "consumers/data-events-queue");
        request.Content = new StringContent(body, Encoding.UTF8, "application/json");
        request.Headers.Add("ResourceType", resourceType);

        return await httpClient.SendAsync(request, cancellationToken);
    }

    public void Dispose()
    {
        httpClient.Dispose();
    }
}
