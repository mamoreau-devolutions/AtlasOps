namespace AtlasOps.Features.Edge.EdgeApplicationRecovery;

public sealed record EdgeApplicationRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);