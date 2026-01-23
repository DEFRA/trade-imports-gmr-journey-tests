using AutoFixture;
using AutoFixture.Dsl;
using Defra.TradeImportsDataApi.Domain.Events;
using Defra.TradeImportsDataApi.Domain.Ipaffs;
using TestHelpers;

namespace TestFixtures;

public static class ImportPreNotificationFixtures
{
    public static IPostprocessComposer<ResourceEvent<ImportPreNotification>> ImportPreNotificationResourceEventFixture(
        ImportPreNotification importPreNotification
    )
    {
        return GetFixture()
            .Build<ResourceEvent<ImportPreNotification>>()
            .With(x => x.Resource, importPreNotification)
            .With(x => x.ResourceId, importPreNotification.ReferenceNumber)
            .With(x => x.ResourceType, ResourceEventResourceTypes.ImportPreNotification);
    }

    public static IPostprocessComposer<ImportPreNotification> ImportPreNotificationFixture(string? mrn = null)
    {
        var importPreNotification = GetFixture().Build<ImportPreNotification>();
        if (mrn == null)
        {
            return importPreNotification;
        }
        return importPreNotification
            .With(x => x.ExternalReferences, [new ExternalReference { Reference = mrn, System = "NCTS" }])
            .With(x => x.ReferenceNumber, ChedGenerator.GenerateChed());
    }

    public static IPostprocessComposer<ImportPreNotification> WithInspectionRequired(
        this IPostprocessComposer<ImportPreNotification> notification,
        bool inspectionRequired
    )
    {
        var partTwo = GetFixture()
            .Build<PartTwo>()
            .With(x => x.InspectionRequired, inspectionRequired ? "Required" : "Not required")
            .Create();

        return notification.With(x => x.PartTwo, partTwo);
    }

    public static IPostprocessComposer<ImportPreNotification> WithNctsMrn(
        this IPostprocessComposer<ImportPreNotification> notification,
        string mrn
    )
    {
        var partOne = GetFixture().Build<PartOne>().With(x => x.ProvideCtcMrn, "YES").Create();

        return notification
            .With(x => x.PartOne, partOne)
            .With(x => x.ExternalReferences, [new ExternalReference { Reference = mrn, System = "NCTS" }]);
    }

    private static Fixture GetFixture()
    {
        var fixture = new Fixture();
        fixture.Customize<DateOnly>(o => o.FromFactory((DateTime dt) => DateOnly.FromDateTime(dt)));
        return fixture;
    }
}
