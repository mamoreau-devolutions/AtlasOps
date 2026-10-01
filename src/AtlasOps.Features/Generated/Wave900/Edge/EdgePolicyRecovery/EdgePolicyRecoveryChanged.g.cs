namespace AtlasOps.Features.Edge.EdgePolicyRecovery;

public sealed record EdgePolicyRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);