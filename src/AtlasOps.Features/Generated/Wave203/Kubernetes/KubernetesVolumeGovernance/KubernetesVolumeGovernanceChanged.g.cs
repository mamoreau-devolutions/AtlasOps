namespace AtlasOps.Features.Kubernetes.KubernetesVolumeGovernance;

public sealed record KubernetesVolumeGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);