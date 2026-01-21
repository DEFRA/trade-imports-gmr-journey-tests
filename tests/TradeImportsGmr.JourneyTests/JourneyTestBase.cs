using System.Text.Json;
using AwesomeAssertions;
using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsDataApi.Domain.Events;
using Defra.TradeImportsDataApi.Domain.Ipaffs;
using TradeImportsGmr.JourneyTests.Clients.GmrFinder;
using TradeImportsGmr.JourneyTests.Clients.GmrProcessor;

namespace TradeImportsGmr.JourneyTests;

public abstract class JourneyTestBase
{
    protected readonly GmrFinderMessageClient GmrFinderMessageClient = GmrFinderMessageClient.Create();
    protected readonly GmrProcessorMessageClient GmrProcessorMessageClient = GmrProcessorMessageClient.Create();

    protected async Task SendCustomsDeclarationToBothServices(
        ResourceEvent<CustomsDeclaration> customsDeclarationEvent,
        CancellationToken cancellationToken
    )
    {
        var serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        var body = JsonSerializer.Serialize(customsDeclarationEvent, serializerOptions);

        var finderResult = await GmrFinderMessageClient.SendDataEventAsync(
            "CustomsDeclaration",
            body,
            cancellationToken
        );
        finderResult.IsSuccessStatusCode.Should().BeTrue();

        var processorResult = await GmrProcessorMessageClient.SendDataEventAsync(
            "CustomsDeclaration",
            body,
            cancellationToken
        );
        processorResult.IsSuccessStatusCode.Should().BeTrue();
    }

    protected async Task SendImportPreNotificationToBothServices(
        ResourceEvent<ImportPreNotification> importPreNotificationEvent,
        CancellationToken cancellationToken
    )
    {
        var serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        var body = JsonSerializer.Serialize(importPreNotificationEvent, serializerOptions);

        var finderResult = await GmrFinderMessageClient.SendDataEventAsync(
            "ImportPreNotification",
            body,
            cancellationToken
        );
        finderResult.IsSuccessStatusCode.Should().BeTrue();

        var processorResult = await GmrProcessorMessageClient.SendDataEventAsync(
            "ImportPreNotification",
            body,
            cancellationToken
        );
        processorResult.IsSuccessStatusCode.Should().BeTrue();
    }
}
