namespace AtlasOps.Features.Observability.ObservabilityDashboardProvisioning;

public sealed record UpdateObservabilityDashboardProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);