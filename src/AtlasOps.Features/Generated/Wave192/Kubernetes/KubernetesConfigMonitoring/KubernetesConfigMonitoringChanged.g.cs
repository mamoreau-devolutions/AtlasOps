namespace AtlasOps.Features.Kubernetes.KubernetesConfigMonitoring;

public sealed record KubernetesConfigMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);