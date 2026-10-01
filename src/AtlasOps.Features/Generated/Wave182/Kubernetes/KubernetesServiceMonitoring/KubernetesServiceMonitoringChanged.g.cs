namespace AtlasOps.Features.Kubernetes.KubernetesServiceMonitoring;

public sealed record KubernetesServiceMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);