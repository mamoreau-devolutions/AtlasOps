namespace AtlasOps.Features.Kubernetes.KubernetesServiceMonitoring;

public sealed record UpdateKubernetesServiceMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);