using AwesomeAssertions;
using TradeImportsGmr.JourneyTests.Clients.GmrFinder;

namespace TradeImportsGmr.JourneyTests.GTO;

public class HoldAndReleaseTransitTest
{
    private readonly GmrFinderMessageClient _gmrFinderMessageClient = GmrFinderMessageClient.Create();

    [Fact]
    public async Task ItCanSendAMessage()
    {
        var fixturePath = Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "..",
            "..",
            "..",
            "TestFixtures",
            "Fixtures",
            "customs-declaration.json"
        );
        var requestBody = await File.ReadAllTextAsync(fixturePath, TestContext.Current.CancellationToken);

        var result = await _gmrFinderMessageClient.SendDataEventAsync(
            "CustomsDeclaration",
            requestBody,
            TestContext.Current.CancellationToken
        );

        result.IsSuccessStatusCode.Should().BeTrue();
    }
}

// send message to input sqs queue trade_imports_data_upserted_gmr_finder
// assert message gets to output sqs queue trade_imports_matched_gmrs_processor_gto (don't forget to match on gvms stub)

// GTO - Places or removes holds on GMRs via GVMS depending on the InspectionRequired ImportPreNotification status
