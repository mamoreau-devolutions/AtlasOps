namespace AtlasOps.Features.Kubernetes.KubernetesOperatorOptimization;

public sealed record UpdateKubernetesOperatorOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);