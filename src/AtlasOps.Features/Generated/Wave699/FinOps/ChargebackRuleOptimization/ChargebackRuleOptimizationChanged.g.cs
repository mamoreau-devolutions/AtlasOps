namespace AtlasOps.Features.FinOps.ChargebackRuleOptimization;

public sealed record ChargebackRuleOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);