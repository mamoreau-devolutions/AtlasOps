namespace AtlasOps.Features.Kubernetes.KubernetesSecretGovernance;

public sealed record UpdateKubernetesSecretGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);