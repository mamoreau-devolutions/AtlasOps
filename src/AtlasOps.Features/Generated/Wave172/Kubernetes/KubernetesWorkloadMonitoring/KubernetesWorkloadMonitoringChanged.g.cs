namespace AtlasOps.Features.Kubernetes.KubernetesWorkloadMonitoring;

public sealed record KubernetesWorkloadMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);