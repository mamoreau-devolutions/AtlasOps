namespace AtlasOps.Features.Observability.TraceSourceOptimization;

public sealed record UpdateTraceSourceOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);