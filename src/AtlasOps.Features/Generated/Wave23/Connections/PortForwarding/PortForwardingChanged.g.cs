namespace AtlasOps.Features.Connections.PortForwarding;

public sealed record PortForwardingChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);