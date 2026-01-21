using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace TradeImportsGmr.JourneyTests.Clients.GmrProcessor;

public enum GmrProcessorMessageType
{
    GvmsHoldRequest,
    IpaffsUpdatedTimeOfArrivalMessage,
}

public sealed class GmrProcessorMessageClient(HttpClient httpClient) : IDisposable
{
    public static GmrProcessorMessageClient Create()
    {
        var options = GmrProcessorMessageClientOptions.FromConfiguration();

        var httpClient = new HttpClient { BaseAddress = options.BaseUri, Timeout = options.Timeout };
        if (!string.IsNullOrWhiteSpace(options.ApiKey))
        {
            httpClient.DefaultRequestHeaders.Add("X-Api-Key", options.ApiKey);
        }
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Basic",
            Convert.ToBase64String(Encoding.UTF8.GetBytes(options.BasicAuth))
        );

        return new GmrProcessorMessageClient(httpClient);
    }

    public async Task<HttpResponseMessage> GetMessageAsync(
        GmrProcessorMessageType messageType,
        CancellationToken cancellationToken = default
    )
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"messages?messageType={messageType}");

        return await httpClient.SendAsync(request, cancellationToken);
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
