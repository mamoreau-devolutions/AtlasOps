namespace AtlasOps.Features.Kubernetes.KubernetesSecretMonitoring;

public sealed record KubernetesSecretMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);