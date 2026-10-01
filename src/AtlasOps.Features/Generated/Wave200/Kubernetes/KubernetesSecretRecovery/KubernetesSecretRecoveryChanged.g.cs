namespace AtlasOps.Features.Kubernetes.KubernetesSecretRecovery;

public sealed record KubernetesSecretRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);