namespace AtlasOps.Features.Kubernetes.KubernetesOperatorGovernance;

public sealed record KubernetesOperatorGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);