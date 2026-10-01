namespace AtlasOps.Features.Kubernetes.KubernetesClusterMonitoring;

public sealed record KubernetesClusterMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);