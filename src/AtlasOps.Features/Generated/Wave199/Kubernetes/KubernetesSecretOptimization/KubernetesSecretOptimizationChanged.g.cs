namespace AtlasOps.Features.Kubernetes.KubernetesSecretOptimization;

public sealed record KubernetesSecretOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);