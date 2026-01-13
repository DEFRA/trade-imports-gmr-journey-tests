using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace TradeImportsGmr.JourneyTests.Clients.GmrProcessor;

public sealed record GmrProcessorMessageClientOptions
{
    [Required]
    public string BaseUrl { get; init; } = "http://localhost:3001/";

    public int TimeoutSeconds { get; init; } = 30;

    public string ApiKey { get; init; } = string.Empty;

    [Required]
    public string BasicAuth { get; init; } = string.Empty;

    public Uri BaseUri => new(BaseUrl);

    public TimeSpan Timeout => TimeSpan.FromSeconds(TimeoutSeconds);

    public static GmrProcessorMessageClientOptions FromConfiguration()
    {
        var configuration = BuildConfiguration();

        var services = new ServiceCollection();
        services
            .AddOptions<GmrProcessorMessageClientOptions>()
            .Bind(configuration.GetSection("GmrProcessor"))
            .ValidateDataAnnotations()
            .Validate(
                options => Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out _) && options.BaseUrl.EndsWith('/'),
                "GmrProcessor:BaseUrl must be an absolute URI and end with a trailing slash (for example http://localhost:3001/)."
            );

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<GmrProcessorMessageClientOptions>>();
        return options.Value;
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();
    }
}
