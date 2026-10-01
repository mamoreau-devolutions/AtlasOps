namespace AtlasOps.Features.Kubernetes.KubernetesClusterOptimization;

public sealed record KubernetesClusterOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);