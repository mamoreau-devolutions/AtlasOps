namespace AtlasOps.Features.Kubernetes.KubernetesVolumeMonitoring;

public sealed record KubernetesVolumeMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);