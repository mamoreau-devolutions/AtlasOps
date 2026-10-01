namespace AtlasOps.Features.Observability.LogSourceProvisioning;

public sealed record UpdateLogSourceProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);