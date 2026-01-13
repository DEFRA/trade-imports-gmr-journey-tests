using AwesomeAssertions;
using TradeImportsGmr.JourneyTests.Clients.GmrProcessor;

namespace TradeImportsGmr.JourneyTests.GTO;

public class GmrProcessorGtoTest
{
    private readonly GmrProcessorMessageClient _gmrProcessorMessageClient = GmrProcessorMessageClient.Create();

    [Fact]
    public async Task ItCanRetrieveGvmsHoldRequestMessage()
    {
        var result = await _gmrProcessorMessageClient.GetMessageAsync(
            GmrProcessorMessageType.GvmsHoldRequest,
            TestContext.Current.CancellationToken
        );

        result.IsSuccessStatusCode.Should().BeTrue();
    }
}
