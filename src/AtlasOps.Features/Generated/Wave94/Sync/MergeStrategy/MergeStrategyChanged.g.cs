namespace AtlasOps.Features.Sync.MergeStrategy;

public sealed record MergeStrategyChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);