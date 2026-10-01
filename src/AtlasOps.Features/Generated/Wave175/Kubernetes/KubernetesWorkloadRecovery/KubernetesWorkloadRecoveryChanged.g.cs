namespace AtlasOps.Features.Kubernetes.KubernetesWorkloadRecovery;

public sealed record KubernetesWorkloadRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);