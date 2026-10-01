namespace AtlasOps.Features.Observability.MetricSourceProvisioning;

public sealed record UpdateMetricSourceProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);