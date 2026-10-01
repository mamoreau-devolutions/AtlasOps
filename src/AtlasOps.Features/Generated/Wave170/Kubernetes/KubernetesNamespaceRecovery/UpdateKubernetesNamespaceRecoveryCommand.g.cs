namespace AtlasOps.Features.Kubernetes.KubernetesNamespaceRecovery;

public sealed record UpdateKubernetesNamespaceRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);