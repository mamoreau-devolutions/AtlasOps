namespace AtlasOps.Features.FinOps.CostAnomalyRecovery;

public sealed record UpdateCostAnomalyRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);