namespace AtlasOps.Features.Inventory.ReconciliationPlan;

public sealed record ReconciliationPlanChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);