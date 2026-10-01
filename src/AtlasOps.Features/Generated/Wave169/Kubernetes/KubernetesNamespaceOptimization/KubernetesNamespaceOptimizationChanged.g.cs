namespace AtlasOps.Features.Kubernetes.KubernetesNamespaceOptimization;

public sealed record KubernetesNamespaceOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);