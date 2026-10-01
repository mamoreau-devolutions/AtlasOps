namespace AtlasOps.Features.Kubernetes.KubernetesWorkloadRecovery;

public sealed record UpdateKubernetesWorkloadRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);