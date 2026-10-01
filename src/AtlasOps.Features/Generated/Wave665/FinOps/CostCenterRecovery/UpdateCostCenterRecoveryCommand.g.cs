namespace AtlasOps.Features.FinOps.CostCenterRecovery;

public sealed record UpdateCostCenterRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);