namespace AtlasOps.Features.Kubernetes.KubernetesConfigOptimization;

public sealed record KubernetesConfigOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);