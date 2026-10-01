namespace AtlasOps.Features.Kubernetes.KubernetesWorkloadOptimization;

public sealed record KubernetesWorkloadOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);