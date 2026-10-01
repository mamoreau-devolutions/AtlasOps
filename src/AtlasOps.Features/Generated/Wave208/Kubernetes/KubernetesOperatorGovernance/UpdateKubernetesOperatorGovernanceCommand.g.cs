namespace AtlasOps.Features.Kubernetes.KubernetesOperatorGovernance;

public sealed record UpdateKubernetesOperatorGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);