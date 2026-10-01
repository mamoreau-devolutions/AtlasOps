namespace AtlasOps.Features.Observability.MetricAlertProvisioning;

public sealed record UpdateMetricAlertProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);