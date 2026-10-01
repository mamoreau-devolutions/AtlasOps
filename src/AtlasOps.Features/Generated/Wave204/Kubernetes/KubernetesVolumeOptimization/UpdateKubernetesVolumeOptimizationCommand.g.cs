namespace AtlasOps.Features.Kubernetes.KubernetesVolumeOptimization;

public sealed record UpdateKubernetesVolumeOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);