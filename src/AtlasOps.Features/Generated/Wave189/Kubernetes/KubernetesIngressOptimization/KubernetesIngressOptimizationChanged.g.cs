namespace AtlasOps.Features.Kubernetes.KubernetesIngressOptimization;

public sealed record KubernetesIngressOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);