namespace AtlasOps.Features.Kubernetes.KubernetesConfigGovernance;

public sealed record KubernetesConfigGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);