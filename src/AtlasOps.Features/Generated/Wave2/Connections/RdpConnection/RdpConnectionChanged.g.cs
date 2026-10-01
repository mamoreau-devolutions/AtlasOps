namespace AtlasOps.Features.Connections.RdpConnection;

public sealed record RdpConnectionChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);