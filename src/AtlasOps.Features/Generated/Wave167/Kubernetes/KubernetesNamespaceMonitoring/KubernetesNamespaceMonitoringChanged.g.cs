namespace AtlasOps.Features.Kubernetes.KubernetesNamespaceMonitoring;

public sealed record KubernetesNamespaceMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);