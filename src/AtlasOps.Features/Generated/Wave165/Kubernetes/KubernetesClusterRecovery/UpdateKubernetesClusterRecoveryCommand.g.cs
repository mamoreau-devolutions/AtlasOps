namespace AtlasOps.Features.Kubernetes.KubernetesClusterRecovery;

public sealed record UpdateKubernetesClusterRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);