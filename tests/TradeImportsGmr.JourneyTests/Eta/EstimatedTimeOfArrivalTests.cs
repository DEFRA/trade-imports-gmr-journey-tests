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

    [Fact]
    public async Task WhenTheTimeOfArrivalChanges_ItSendsMultipleEtaMessageUpdates()
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

        var firstResult = await AsyncWaiter.WaitForAsync(
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

        firstResult
            .Should()
            .NotBeNull($"Failed to find first ETA message with reference number {chedReference} and MRN {mrn}");

        var firstMessageBody = JsonSerializer.Deserialize<IpaffsUpdatedTimeOfArrivalMessage>(firstResult!.MessageBody);
        var firstTimestamp = firstMessageBody!.LocalDateTimeOfArrival;

        var secondResult = await AsyncWaiter.WaitForAsync(
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
                        && messageBody.Mrn == mrn
                        && messageBody.LocalDateTimeOfArrival != firstTimestamp;
                });
            },
            TestContext.Current.CancellationToken
        );

        secondResult
            .Should()
            .NotBeNull($"Failed to find second ETA message with reference number {chedReference} and MRN {mrn}");

        var secondMessageBody = JsonSerializer.Deserialize<IpaffsUpdatedTimeOfArrivalMessage>(secondResult.MessageBody);
        secondMessageBody!.ReferenceNumber.Should().Be(chedReference);
        secondMessageBody.Mrn.Should().Be(mrn);
        secondMessageBody.LocalDateTimeOfArrival.Should().NotBe(firstTimestamp);
    }
}
