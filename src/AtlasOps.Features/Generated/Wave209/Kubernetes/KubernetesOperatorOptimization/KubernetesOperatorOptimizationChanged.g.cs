namespace AtlasOps.Features.Kubernetes.KubernetesOperatorOptimization;

public sealed record KubernetesOperatorOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);