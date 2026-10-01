namespace AtlasOps.Features.Delivery.ReleaseEnvironmentRecovery;

public sealed record ReleaseEnvironmentRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);