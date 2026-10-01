namespace AtlasOps.Features.Kubernetes.KubernetesServiceRecovery;

public sealed record UpdateKubernetesServiceRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);