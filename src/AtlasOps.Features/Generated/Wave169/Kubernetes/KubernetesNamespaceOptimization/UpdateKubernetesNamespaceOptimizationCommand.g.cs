namespace AtlasOps.Features.Kubernetes.KubernetesNamespaceOptimization;

public sealed record UpdateKubernetesNamespaceOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);