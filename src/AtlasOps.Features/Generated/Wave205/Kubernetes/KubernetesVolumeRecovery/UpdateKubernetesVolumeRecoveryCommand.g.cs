namespace AtlasOps.Features.Kubernetes.KubernetesVolumeRecovery;

public sealed record UpdateKubernetesVolumeRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);