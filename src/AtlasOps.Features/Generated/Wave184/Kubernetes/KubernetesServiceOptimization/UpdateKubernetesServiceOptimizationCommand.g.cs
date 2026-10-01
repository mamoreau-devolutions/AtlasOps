namespace AtlasOps.Features.Kubernetes.KubernetesServiceOptimization;

public sealed record UpdateKubernetesServiceOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);