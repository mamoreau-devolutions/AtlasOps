namespace AtlasOps.Features.Data.DataPipelineRecovery;

public sealed record DataPipelineRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);