namespace AtlasOps.Features.FinOps.SpendForecastProvisioning;

public sealed record UpdateSpendForecastProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);