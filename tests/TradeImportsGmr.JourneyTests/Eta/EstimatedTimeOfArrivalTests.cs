using System.Net.Http.Json;
using System.Text.Json;
using AutoFixture;
using AwesomeAssertions;
using TestFixtures;
using TestHelpers;
using TradeImportsGmr.JourneyTests.Clients.GmrProcessor;
using TradeImportsGmr.JourneyTests.Utils;

namespace TradeImportsGmr.JourneyTests.ETA;

public class EstimatedTimeOfArrivalTests : JourneyTestBase
{
    [Fact]
    public async Task ItCanSendMatchedGmrEventsAndReceiveEtaMessage()
    {
        var mrn = MrnGenerator.GenerateMrn();
        var chedReference = ChedGenerator.GenerateChed();

        var customsDeclaration = CustomsDeclarationFixtures
            .CustomsDeclarationFixture()
            .With(
                x => x.ClearanceDecision,
                CustomsDeclarationFixtures.ClearanceDecisionFixture([chedReference]).Create()
            )
            .Create();

        var customsDeclarationEvent = CustomsDeclarationFixtures
            .CustomsDeclarationResourceEventFixture(customsDeclaration)
            .With(x => x.ResourceId, mrn)
            .Create();

        var importPreNotification = ImportPreNotificationFixtures
            .ImportPreNotificationFixture(mrn)
            .With(x => x.ReferenceNumber, chedReference)
            .Create();

        var importPreNotificationEvent = ImportPreNotificationFixtures
            .ImportPreNotificationResourceEventFixture(importPreNotification)
            .Create();

        await SendCustomsDeclarationToBothServices(customsDeclarationEvent, TestContext.Current.CancellationToken);

        await SendImportPreNotificationToBothServices(
            importPreNotificationEvent,
            TestContext.Current.CancellationToken
        );

        var result = await AsyncWaiter.WaitForAsync(
            async () =>
            {
                var messages = await GmrProcessorMessageClient.GetMessageAsync(
                    GmrProcessorMessageType.IpaffsUpdatedTimeOfArrivalMessage,
                    TestContext.Current.CancellationToken
                );
                messages.IsSuccessStatusCode.Should().BeTrue();

                var parsed = await messages.Content.ReadFromJsonAsync<List<MessageAudit>>(
                    TestContext.Current.CancellationToken
                );

                return parsed?.FirstOrDefault(p =>
                {
                    var messageBody = JsonSerializer.Deserialize<IpaffsUpdatedTimeOfArrivalMessage>(p.MessageBody);
                    return messageBody != null
                        && messageBody.ReferenceNumber == chedReference
                        && messageBody.Mrn == mrn;
                });
            },
            TestContext.Current.CancellationToken
        );

        result.Should().NotBeNull();
    }
}
