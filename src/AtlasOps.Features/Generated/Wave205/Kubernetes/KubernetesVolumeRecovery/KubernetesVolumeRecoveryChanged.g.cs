namespace AtlasOps.Features.Kubernetes.KubernetesVolumeRecovery;

public sealed record KubernetesVolumeRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);