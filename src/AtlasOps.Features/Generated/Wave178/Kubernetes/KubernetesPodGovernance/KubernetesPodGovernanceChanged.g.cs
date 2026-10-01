namespace AtlasOps.Features.Kubernetes.KubernetesPodGovernance;

public sealed record KubernetesPodGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);