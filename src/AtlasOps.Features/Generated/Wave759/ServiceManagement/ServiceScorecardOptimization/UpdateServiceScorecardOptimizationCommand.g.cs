namespace AtlasOps.Features.ServiceManagement.ServiceScorecardOptimization;

public sealed record UpdateServiceScorecardOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);