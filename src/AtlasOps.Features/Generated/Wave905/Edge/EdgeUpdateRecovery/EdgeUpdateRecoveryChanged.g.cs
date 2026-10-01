namespace AtlasOps.Features.Edge.EdgeUpdateRecovery;

public sealed record EdgeUpdateRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);