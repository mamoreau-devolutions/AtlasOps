namespace AtlasOps.Features.Kubernetes.KubernetesNamespaceRecovery;

public sealed record KubernetesNamespaceRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);