namespace AtlasOps.Features.Kubernetes.KubernetesClusterGovernance;

public sealed record KubernetesClusterGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);