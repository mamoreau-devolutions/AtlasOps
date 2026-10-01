namespace AtlasOps.Features.ServiceManagement.ServiceScorecardProvisioning;

public sealed record UpdateServiceScorecardProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);