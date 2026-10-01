namespace AtlasOps.Features.Kubernetes.KubernetesSecretGovernance;

public sealed record KubernetesSecretGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);