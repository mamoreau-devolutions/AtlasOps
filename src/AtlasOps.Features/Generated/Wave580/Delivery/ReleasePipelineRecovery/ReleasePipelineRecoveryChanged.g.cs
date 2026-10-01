namespace AtlasOps.Features.Delivery.ReleasePipelineRecovery;

public sealed record ReleasePipelineRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);