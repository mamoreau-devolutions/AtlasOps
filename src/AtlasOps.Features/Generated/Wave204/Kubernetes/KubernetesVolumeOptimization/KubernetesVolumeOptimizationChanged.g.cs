namespace AtlasOps.Features.Kubernetes.KubernetesVolumeOptimization;

public sealed record KubernetesVolumeOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);