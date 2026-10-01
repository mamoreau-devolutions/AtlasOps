namespace AtlasOps.Features.Automation.ParallelStage;

public sealed record ParallelStageChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);