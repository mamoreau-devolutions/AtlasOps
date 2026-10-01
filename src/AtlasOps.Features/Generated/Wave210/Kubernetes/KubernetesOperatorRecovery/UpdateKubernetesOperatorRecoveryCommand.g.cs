namespace AtlasOps.Features.Kubernetes.KubernetesOperatorRecovery;

public sealed record UpdateKubernetesOperatorRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);