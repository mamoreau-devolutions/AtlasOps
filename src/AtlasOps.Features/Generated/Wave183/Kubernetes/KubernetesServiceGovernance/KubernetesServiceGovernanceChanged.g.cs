namespace AtlasOps.Features.Kubernetes.KubernetesServiceGovernance;

public sealed record KubernetesServiceGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);