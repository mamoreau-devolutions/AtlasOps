namespace AtlasOps.Features.Kubernetes.KubernetesNamespaceGovernance;

public sealed record UpdateKubernetesNamespaceGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);