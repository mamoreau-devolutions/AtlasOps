namespace AtlasOps.Features.Delivery.BuildArtifactRecovery;

public sealed record BuildArtifactRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);