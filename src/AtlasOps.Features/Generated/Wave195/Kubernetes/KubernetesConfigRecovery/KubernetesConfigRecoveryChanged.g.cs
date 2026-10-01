namespace AtlasOps.Features.Kubernetes.KubernetesConfigRecovery;

public sealed record KubernetesConfigRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);