namespace AtlasOps.Features.Kubernetes.KubernetesServiceGovernance;

public sealed record UpdateKubernetesServiceGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);