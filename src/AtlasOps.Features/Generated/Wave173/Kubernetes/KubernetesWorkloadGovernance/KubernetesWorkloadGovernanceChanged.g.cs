namespace AtlasOps.Features.Kubernetes.KubernetesWorkloadGovernance;

public sealed record KubernetesWorkloadGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);