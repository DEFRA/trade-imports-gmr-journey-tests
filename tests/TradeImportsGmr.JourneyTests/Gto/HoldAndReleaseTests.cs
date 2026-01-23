using System.Net.Http.Json;
using System.Text.Json;
using AutoFixture;
using AwesomeAssertions;
using TestFixtures;
using TestHelpers;
using TradeImportsGmr.JourneyTests.Clients.GmrProcessor;
using TradeImportsGmr.JourneyTests.Utils;

namespace TradeImportsGmr.JourneyTests.GTO;

public class HoldAndReleaseTransitTests : JourneyTestBase
{
    [Fact]
    public async Task GivenAnImportPreNotificationRequiringAnInspection_AHoldIsPlaced_AndThenReleased()
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
            .WithNctsMrn(mrn)
            .WithInspectionRequired(true)
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
                    GmrProcessorMessageType.GvmsHoldRequest,
                    TestContext.Current.CancellationToken
                );
                messages.IsSuccessStatusCode.Should().BeTrue();

                var parsed = await messages.Content.ReadFromJsonAsync<List<MessageAudit>>(
                    TestContext.Current.CancellationToken
                );

                return parsed?.FirstOrDefault(p =>
                {
                    var messageBody = JsonSerializer.Deserialize<GvmsHoldRecord>(p.MessageBody);
                    return messageBody != null && messageBody.Mrns.Contains(mrn) && messageBody.Hold;
                });
            },
            TestContext.Current.CancellationToken
        );

        result.Should().NotBeNull($"Failed to place hold on MRN {mrn} with CHED {chedReference}");

        var importPreNotificationReleased = ImportPreNotificationFixtures
            .ImportPreNotificationFixture(mrn)
            .With(x => x.ReferenceNumber, chedReference)
            .WithNctsMrn(mrn)
            .WithInspectionRequired(false)
            .Create();

        var importPreNotificationEventReleased = ImportPreNotificationFixtures
            .ImportPreNotificationResourceEventFixture(importPreNotificationReleased)
            .Create();

        await SendImportPreNotificationToBothServices(
            importPreNotificationEventReleased,
            TestContext.Current.CancellationToken
        );

        var resultReleased = await AsyncWaiter.WaitForAsync(
            async () =>
            {
                var messages = await GmrProcessorMessageClient.GetMessageAsync(
                    GmrProcessorMessageType.GvmsHoldRequest,
                    TestContext.Current.CancellationToken
                );
                messages.IsSuccessStatusCode.Should().BeTrue();

                var parsed = await messages.Content.ReadFromJsonAsync<List<MessageAudit>>(
                    TestContext.Current.CancellationToken
                );

                return parsed?.FirstOrDefault(p =>
                {
                    var messageBody = JsonSerializer.Deserialize<GvmsHoldRecord>(p.MessageBody);
                    return messageBody != null && messageBody.Mrns.Contains(mrn) && !messageBody.Hold;
                });
            },
            TestContext.Current.CancellationToken
        );

        resultReleased.Should().NotBeNull($"Failed to release hold on MRN {mrn} with CHED {chedReference}");
    }
}
