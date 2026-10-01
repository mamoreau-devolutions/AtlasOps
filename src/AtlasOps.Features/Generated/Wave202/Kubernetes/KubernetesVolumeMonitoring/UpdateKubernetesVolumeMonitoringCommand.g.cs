namespace AtlasOps.Features.Kubernetes.KubernetesVolumeMonitoring;

public sealed record UpdateKubernetesVolumeMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);