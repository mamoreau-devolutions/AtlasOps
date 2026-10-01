namespace AtlasOps.Features.BusinessContinuity.ContinuityPlanProvisioning;

public sealed record UpdateContinuityPlanProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);