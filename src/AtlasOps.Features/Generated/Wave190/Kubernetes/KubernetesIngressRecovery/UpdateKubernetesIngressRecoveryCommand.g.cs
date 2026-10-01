namespace AtlasOps.Features.Kubernetes.KubernetesIngressRecovery;

public sealed record UpdateKubernetesIngressRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);