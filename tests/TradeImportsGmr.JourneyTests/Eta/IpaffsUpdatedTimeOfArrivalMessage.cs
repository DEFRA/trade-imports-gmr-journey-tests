namespace TradeImportsGmr.JourneyTests.ETA;

public class IpaffsUpdatedTimeOfArrivalMessage
{
    public required string ReferenceNumber { get; init; }
    public required string EntryReference { get; init; }
    public required DateTime LocalDateTimeOfArrival { get; init; }
}
