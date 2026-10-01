namespace AtlasOps.Features.Connections.ConnectionPool;

public sealed record ConnectionPoolChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);