namespace AtlasOps.Features.Kubernetes.KubernetesClusterMonitoring;

public sealed record UpdateKubernetesClusterMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);