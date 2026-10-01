namespace AtlasOps.Features.Automation.RollbackPlan;

public sealed record RollbackPlanChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);