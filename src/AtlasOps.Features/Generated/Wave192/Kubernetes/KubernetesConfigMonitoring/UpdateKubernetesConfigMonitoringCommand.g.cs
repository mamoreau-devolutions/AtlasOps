namespace AtlasOps.Features.Kubernetes.KubernetesConfigMonitoring;

public sealed record UpdateKubernetesConfigMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);