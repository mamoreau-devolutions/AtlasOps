namespace AtlasOps.Features.Observability.ObservabilityRetentionOptimization;

public sealed record UpdateObservabilityRetentionOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);