namespace AtlasOps.Features.FinOps.ChargebackRuleOptimization;

public sealed record UpdateChargebackRuleOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);