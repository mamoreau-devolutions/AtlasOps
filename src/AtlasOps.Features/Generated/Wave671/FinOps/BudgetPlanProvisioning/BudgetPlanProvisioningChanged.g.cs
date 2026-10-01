namespace AtlasOps.Features.FinOps.BudgetPlanProvisioning;

public sealed record BudgetPlanProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);