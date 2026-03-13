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

        var importPreNotification = ImportPreNotificationFixtures
            .ImportPreNotificationFixture(mrn)
            .With(x => x.ReferenceNumber, chedReference)
            .WithNctsMrn(mrn)
            .WithInspectionRequired(true)
            .Create();

        var importPreNotificationEvent = ImportPreNotificationFixtures
            .ImportPreNotificationResourceEventFixture(importPreNotification)
            .Create();

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

    [Fact]
    public async Task GivenMultipleImportPreNotificationsWithIdenticalMrnRequiringAnInspection_AHoldIsPlaced_AndThenReleased()
    {
        var mrn = "26GB123456789AB017";
        var chedReference1 = ChedGenerator.GenerateChed();
        var chedReference2 = ChedGenerator.GenerateChed();

        var importPreNotification1 = ImportPreNotificationFixtures
            .ImportPreNotificationFixture(mrn)
            .With(x => x.ReferenceNumber, chedReference1)
            .WithNctsMrn(mrn)
            .WithInspectionRequired(true)
            .Create();

        var importPreNotificationEvent1 = ImportPreNotificationFixtures
            .ImportPreNotificationResourceEventFixture(importPreNotification1)
            .Create();

        var importPreNotification2 = ImportPreNotificationFixtures
            .ImportPreNotificationFixture(mrn)
            .With(x => x.ReferenceNumber, chedReference2)
            .WithNctsMrn(mrn)
            .WithInspectionRequired(true)
            .Create();

        var importPreNotificationEvent2 = ImportPreNotificationFixtures
            .ImportPreNotificationResourceEventFixture(importPreNotification2)
            .Create();

        await SendImportPreNotificationToBothServices(
            importPreNotificationEvent1,
            TestContext.Current.CancellationToken
        );

        await SendImportPreNotificationToBothServices(
            importPreNotificationEvent2,
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

        result.Should().NotBeNull($"Failed to place hold on MRN {mrn} with CHED {chedReference1}");
        result.Should().NotBeNull($"Failed to place hold on MRN {mrn} with CHED {chedReference2}");

        var importPreNotificationReleased1 = ImportPreNotificationFixtures
            .ImportPreNotificationFixture(mrn)
            .With(x => x.ReferenceNumber, chedReference1)
            .WithNctsMrn(mrn)
            .WithInspectionRequired(false)
            .Create();

        var importPreNotificationEventReleased1 = ImportPreNotificationFixtures
            .ImportPreNotificationResourceEventFixture(importPreNotificationReleased1)
            .Create();

        var importPreNotificationReleased2 = ImportPreNotificationFixtures
            .ImportPreNotificationFixture(mrn)
            .With(x => x.ReferenceNumber, chedReference2)
            .WithNctsMrn(mrn)
            .WithInspectionRequired(false)
            .Create();

        var importPreNotificationEventReleased2 = ImportPreNotificationFixtures
            .ImportPreNotificationResourceEventFixture(importPreNotificationReleased2)
            .Create();

        await SendImportPreNotificationToBothServices(
            importPreNotificationEventReleased1,
            TestContext.Current.CancellationToken
        );

        await SendImportPreNotificationToBothServices(
            importPreNotificationEventReleased2,
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

        resultReleased.Should().NotBeNull($"Failed to release hold on MRN {mrn} with CHED {chedReference1}");
        resultReleased.Should().NotBeNull($"Failed to release hold on MRN {mrn} with CHED {chedReference2}");
    }
}
