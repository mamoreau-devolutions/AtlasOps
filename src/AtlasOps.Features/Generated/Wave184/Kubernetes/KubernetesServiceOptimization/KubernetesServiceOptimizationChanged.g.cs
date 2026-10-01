namespace AtlasOps.Features.Kubernetes.KubernetesServiceOptimization;

public sealed record KubernetesServiceOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);