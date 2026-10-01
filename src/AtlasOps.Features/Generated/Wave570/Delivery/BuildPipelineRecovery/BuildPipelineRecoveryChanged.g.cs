namespace AtlasOps.Features.Delivery.BuildPipelineRecovery;

public sealed record BuildPipelineRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);