namespace AtlasOps.Features.FinOps.CostCenterProvisioning;

public sealed record UpdateCostCenterProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);