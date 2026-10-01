namespace AtlasOps.Features.Connections.DatabaseConnection;

public sealed record DatabaseConnectionChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);