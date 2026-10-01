namespace AtlasOps.Features.Kubernetes.KubernetesPodMonitoring;

public sealed record KubernetesPodMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);