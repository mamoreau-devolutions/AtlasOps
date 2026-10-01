namespace AtlasOps.Features.Kubernetes.KubernetesOperatorMonitoring;

public sealed record KubernetesOperatorMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);