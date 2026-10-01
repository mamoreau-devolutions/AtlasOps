namespace AtlasOps.Features.Automation.CanaryRollout;

public sealed record CanaryRolloutChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);