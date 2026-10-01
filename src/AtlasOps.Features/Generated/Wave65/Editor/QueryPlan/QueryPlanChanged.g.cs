namespace AtlasOps.Features.Editor.QueryPlan;

public sealed record QueryPlanChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);