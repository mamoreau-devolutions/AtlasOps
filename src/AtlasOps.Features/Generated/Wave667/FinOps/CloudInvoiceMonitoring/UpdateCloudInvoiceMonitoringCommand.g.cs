namespace AtlasOps.Features.FinOps.CloudInvoiceMonitoring;

public sealed record UpdateCloudInvoiceMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);