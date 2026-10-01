namespace AtlasOps.Features.Kubernetes.KubernetesOperatorRecovery;

public sealed record KubernetesOperatorRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);