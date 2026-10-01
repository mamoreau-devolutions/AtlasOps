namespace AtlasOps.Features.Delivery.ReleaseGateRecovery;

public sealed record ReleaseGateRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);