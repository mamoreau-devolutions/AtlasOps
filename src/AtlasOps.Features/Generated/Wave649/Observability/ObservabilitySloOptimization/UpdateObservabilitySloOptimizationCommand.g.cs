namespace AtlasOps.Features.Observability.ObservabilitySloOptimization;

public sealed record UpdateObservabilitySloOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);