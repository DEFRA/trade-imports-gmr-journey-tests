using System.Text.Json;
using AutoFixture;
using TradeImportsGmr.JourneyTests.Clients.GmrFinder;

namespace TradeImportsGmr.JourneyTests.ETA;

public class EstimatedTimeOfArrivalTests
{
    [Fact]
    public async Task TrueIsTrue()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        var ipaffsNotification = TestFixtures.ImportPreNotificationFixtures.ImportPreNotificationFixture();
        var ipaffsNotificationResourceEvent = TestFixtures
            .ImportPreNotificationFixtures.ImportPreNotificationResourceEventFixture(ipaffsNotification.Create())
            .Create();
        var ipaffsNotificationBody = JsonSerializer.Serialize(
            ipaffsNotificationResourceEvent,
            new JsonSerializerOptions(JsonSerializerDefaults.Web)
        );

        var gmrFinder = GmrFinderMessageClient.Create();
        await gmrFinder.SendDataEventAsync(
            ResourceTypes.ImportPreNotification,
            ipaffsNotificationBody,
            cancellationToken
        );
        Assert.True(true);
    }
}

// send message to input sqs queue trade_imports_data_upserted_gmr_finder
// assert message gets to output sns topic trade_imports_matched_gmrs_gmr_processor_eta (don't forget to match on gvms stub)

// ETA - Provides an estimated time of arrival to Ipaffs when a GMR is marked as Embarked
