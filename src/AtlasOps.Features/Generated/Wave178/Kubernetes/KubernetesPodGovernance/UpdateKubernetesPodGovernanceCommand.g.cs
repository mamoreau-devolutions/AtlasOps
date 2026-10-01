namespace AtlasOps.Features.Kubernetes.KubernetesPodGovernance;

public sealed record UpdateKubernetesPodGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);