namespace AtlasOps.Features.Automation.ExecutionCheckpoint;

public sealed record ExecutionCheckpointChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);