namespace AtlasOps.Features.Observability.TraceSpanOptimization;

public sealed record UpdateTraceSpanOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);