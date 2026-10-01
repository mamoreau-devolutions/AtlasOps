namespace AtlasOps.Features.Kubernetes.KubernetesPodRecovery;

public sealed record KubernetesPodRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);