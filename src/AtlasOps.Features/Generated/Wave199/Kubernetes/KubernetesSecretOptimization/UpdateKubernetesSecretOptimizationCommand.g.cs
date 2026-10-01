namespace AtlasOps.Features.Kubernetes.KubernetesSecretOptimization;

public sealed record UpdateKubernetesSecretOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);