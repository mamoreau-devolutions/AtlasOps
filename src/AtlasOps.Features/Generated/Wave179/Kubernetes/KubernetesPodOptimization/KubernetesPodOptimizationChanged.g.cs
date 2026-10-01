namespace AtlasOps.Features.Kubernetes.KubernetesPodOptimization;

public sealed record KubernetesPodOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);