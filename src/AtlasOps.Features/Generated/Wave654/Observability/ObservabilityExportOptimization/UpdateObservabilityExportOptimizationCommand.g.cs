namespace AtlasOps.Features.Observability.ObservabilityExportOptimization;

public sealed record UpdateObservabilityExportOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);