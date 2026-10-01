namespace AtlasOps.Features.Kubernetes.KubernetesServiceRecovery;

public sealed record KubernetesServiceRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);