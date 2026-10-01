namespace AtlasOps.Features.Kubernetes.KubernetesClusterGovernance;

public sealed record UpdateKubernetesClusterGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);