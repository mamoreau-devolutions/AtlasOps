namespace AtlasOps.Features.FinOps.SavingsPlanProvisioning;

public sealed record UpdateSavingsPlanProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);