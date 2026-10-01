namespace AtlasOps.Features.Kubernetes.KubernetesWorkloadOptimization;

public sealed record UpdateKubernetesWorkloadOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);