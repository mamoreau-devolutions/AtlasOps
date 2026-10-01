namespace AtlasOps.Features.Mobile.MobileComplianceOptimization;

public sealed record UpdateMobileComplianceOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);