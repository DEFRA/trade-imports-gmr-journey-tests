namespace TradeImportsGmr.JourneyTests.GTO;

public record GvmsHoldRecord
{
    public required string GmrId { get; init; }
    public required List<string> Mrns { get; init; }
    public required bool Hold { get; init; }
}
