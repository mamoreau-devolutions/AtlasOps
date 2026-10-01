namespace AtlasOps.Features.Kubernetes.KubernetesNamespaceGovernance;

public sealed record KubernetesNamespaceGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);