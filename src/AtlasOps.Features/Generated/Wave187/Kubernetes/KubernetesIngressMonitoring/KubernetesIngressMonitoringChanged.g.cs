namespace AtlasOps.Features.Kubernetes.KubernetesIngressMonitoring;

public sealed record KubernetesIngressMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);