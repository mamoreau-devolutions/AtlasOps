namespace AtlasOps.Features.Kubernetes.KubernetesWorkloadGovernance;

public sealed record UpdateKubernetesWorkloadGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);