using System.Text.Json;
using AutoFixture;
using AwesomeAssertions;
using TestFixtures;
using TestHelpers;
using TradeImportsGmr.JourneyTests.Clients.GmrFinder;

namespace TradeImportsGmr.JourneyTests.GTO;

public class HoldAndReleaseTransitTests
{
    private readonly GmrFinderMessageClient _gmrFinderMessageClient = GmrFinderMessageClient.Create();

    [Fact]
    public async Task ItCanSendAMessageCustomsDeclaration()
    {
        // Arrange
        var customsDeclaration = CustomsDeclarationFixtures.CustomsDeclarationFixture().Create();

        var resourceEvent = CustomsDeclarationFixtures
            .CustomsDeclarationResourceEventFixture(customsDeclaration)
            .Create();

        var serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        var requestBody = JsonSerializer.Serialize(resourceEvent, serializerOptions);

        // Act
        var result = await _gmrFinderMessageClient.SendDataEventAsync(
            "CustomsDeclaration",
            requestBody,
            TestContext.Current.CancellationToken
        );

        // Assert
        result.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task ItCanSendAMessageImportPreNotification()
    {
        // Arrange
        var mrn = MrnGenerator.GenerateMrn();

        var importPreNotification = ImportPreNotificationFixtures.ImportPreNotificationFixture(mrn).Create();

        var resourceEvent = ImportPreNotificationFixtures
            .ImportPreNotificationResourceEventFixture(importPreNotification)
            .Create();

        var serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        var requestBody = JsonSerializer.Serialize(resourceEvent, serializerOptions);

        // Act
        var result = await _gmrFinderMessageClient.SendDataEventAsync(
            "ImportPreNotification",
            requestBody,
            TestContext.Current.CancellationToken
        );

        // Assert
        result.IsSuccessStatusCode.Should().BeTrue();
    }
}

// send message to input sqs queue trade_imports_data_upserted_gmr_finder
// assert message gets to output sqs queue trade_imports_matched_gmrs_processor_gto (don't forget to match on gvms stub)

// GTO - Places or removes holds on GMRs via GVMS depending on the InspectionRequired ImportPreNotification status
