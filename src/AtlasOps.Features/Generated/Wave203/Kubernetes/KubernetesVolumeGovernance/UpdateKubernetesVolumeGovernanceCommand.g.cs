namespace AtlasOps.Features.Kubernetes.KubernetesVolumeGovernance;

public sealed record UpdateKubernetesVolumeGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);