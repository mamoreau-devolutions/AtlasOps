namespace AtlasOps.Features.Kubernetes.KubernetesIngressGovernance;

public sealed record KubernetesIngressGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);