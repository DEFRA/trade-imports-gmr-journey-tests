using System.Diagnostics.CodeAnalysis;

namespace TradeImportsGmr.JourneyTests.Clients.GmrProcessor;

[ExcludeFromCodeCoverage]
public class MessageAudit
{
    public string Id { get; set; } = null!;
    public required string Direction { get; init; }
    public required string IntegrationType { get; init; }
    public required string Target { get; init; }
    public required string MessageBody { get; init; }
    public required DateTime Timestamp { get; init; }
    public string? MessageType { get; init; }
}
