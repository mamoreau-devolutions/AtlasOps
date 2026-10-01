namespace AtlasOps.Features.Kubernetes.KubernetesClusterRecovery;

public sealed record KubernetesClusterRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);