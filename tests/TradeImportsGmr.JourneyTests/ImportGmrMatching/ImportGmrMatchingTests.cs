using System.Net.Http.Json;
using System.Text.Json;
using AutoFixture;
using AwesomeAssertions;
using TestFixtures;
using TestHelpers;
using TradeImportsGmr.JourneyTests.Clients.GmrProcessor;
using TradeImportsGmr.JourneyTests.Utils;

namespace TradeImportsGmr.JourneyTests.ImportGmrMatching;

public class ImportGmrMatchingTests : JourneyTestBase
{
    [Fact]
    public async Task GivenAnImportAndCustomsDeclarationMatches_ItShouldSendAnImportMatchMessage()
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
                    GmrProcessorMessageType.ImportMatchMessage,
                    TestContext.Current.CancellationToken
                );
                await AssertSuccessStatusCode(messages, "Getting ImportMatchMessage");

                var parsed = await messages.Content.ReadFromJsonAsync<List<MessageAudit>>(
                    TestContext.Current.CancellationToken
                );

                return parsed?.FirstOrDefault(p =>
                {
                    var messageBody = JsonSerializer.Deserialize<ImportMatchMessage>(p.MessageBody);
                    return messageBody is { Match: true } && messageBody.ImportReference == chedReference;
                });
            },
            TestContext.Current.CancellationToken
        );

        result.Should().NotBeNull($"Failed to send an import match message with MRN {mrn} with CHED {chedReference}");
    }
}
