namespace AtlasOps.Features.FinOps.FinOpsReportOptimization;

public sealed record UpdateFinOpsReportOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);