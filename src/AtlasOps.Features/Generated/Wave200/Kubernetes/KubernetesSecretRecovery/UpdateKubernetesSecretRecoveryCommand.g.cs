namespace AtlasOps.Features.Kubernetes.KubernetesSecretRecovery;

public sealed record UpdateKubernetesSecretRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);