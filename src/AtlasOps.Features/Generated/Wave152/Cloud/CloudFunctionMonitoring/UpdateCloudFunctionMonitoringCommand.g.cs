namespace AtlasOps.Features.Cloud.CloudFunctionMonitoring;

public sealed record UpdateCloudFunctionMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);