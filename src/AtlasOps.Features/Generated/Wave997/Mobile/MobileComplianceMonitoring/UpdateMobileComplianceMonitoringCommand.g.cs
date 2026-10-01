namespace AtlasOps.Features.Mobile.MobileComplianceMonitoring;

public sealed record UpdateMobileComplianceMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);