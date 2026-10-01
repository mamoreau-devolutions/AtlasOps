namespace AtlasOps.Features.FinOps.CostAnomalyProvisioning;

public sealed record UpdateCostAnomalyProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);