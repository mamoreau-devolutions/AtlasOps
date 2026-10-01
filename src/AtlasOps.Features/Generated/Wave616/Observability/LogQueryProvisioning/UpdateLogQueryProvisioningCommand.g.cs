namespace AtlasOps.Features.Observability.LogQueryProvisioning;

public sealed record UpdateLogQueryProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);