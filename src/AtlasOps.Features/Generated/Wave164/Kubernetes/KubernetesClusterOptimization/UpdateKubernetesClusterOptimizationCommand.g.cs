namespace AtlasOps.Features.Kubernetes.KubernetesClusterOptimization;

public sealed record UpdateKubernetesClusterOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);