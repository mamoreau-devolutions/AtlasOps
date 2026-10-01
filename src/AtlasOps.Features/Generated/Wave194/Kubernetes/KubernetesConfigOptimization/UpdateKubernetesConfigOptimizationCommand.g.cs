namespace AtlasOps.Features.Kubernetes.KubernetesConfigOptimization;

public sealed record UpdateKubernetesConfigOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);