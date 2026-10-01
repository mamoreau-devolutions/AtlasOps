namespace AtlasOps.Features.FinOps.CostAllocationProvisioning;

public sealed record UpdateCostAllocationProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);