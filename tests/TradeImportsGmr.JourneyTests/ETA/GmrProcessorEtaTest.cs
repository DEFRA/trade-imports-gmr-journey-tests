using AwesomeAssertions;
using TradeImportsGmr.JourneyTests.Clients.GmrProcessor;

namespace TradeImportsGmr.JourneyTests.ETA;

public class GmrProcessorEtaTest
{
    private readonly GmrProcessorMessageClient _gmrProcessorMessageClient = GmrProcessorMessageClient.Create();

    [Fact]
    public async Task ItCanRetrieveIpaffsUpdatedTimeOfArrivalMessage()
    {
        var result = await _gmrProcessorMessageClient.GetMessageAsync(
            GmrProcessorMessageType.IpaffsUpdatedTimeOfArrivalMessage,
            TestContext.Current.CancellationToken
        );

        result.IsSuccessStatusCode.Should().BeTrue();
    }
}
