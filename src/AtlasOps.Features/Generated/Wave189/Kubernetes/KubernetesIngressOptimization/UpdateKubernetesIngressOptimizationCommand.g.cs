namespace AtlasOps.Features.Kubernetes.KubernetesIngressOptimization;

public sealed record UpdateKubernetesIngressOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);