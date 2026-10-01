namespace AtlasOps.Features.Kubernetes.KubernetesIngressGovernance;

public sealed record UpdateKubernetesIngressGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);