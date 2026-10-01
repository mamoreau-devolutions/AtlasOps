namespace AtlasOps.Features.FinOps.CostAllocationRecovery;

public sealed record UpdateCostAllocationRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);