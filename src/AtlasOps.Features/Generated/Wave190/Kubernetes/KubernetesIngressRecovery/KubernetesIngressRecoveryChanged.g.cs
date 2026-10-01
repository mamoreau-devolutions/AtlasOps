namespace AtlasOps.Features.Kubernetes.KubernetesIngressRecovery;

public sealed record KubernetesIngressRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);