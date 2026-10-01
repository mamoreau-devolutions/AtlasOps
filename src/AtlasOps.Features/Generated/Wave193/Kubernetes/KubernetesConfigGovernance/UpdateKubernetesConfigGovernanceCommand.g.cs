namespace AtlasOps.Features.Kubernetes.KubernetesConfigGovernance;

public sealed record UpdateKubernetesConfigGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);