namespace AtlasOps.Features.Kubernetes.KubernetesIngressMonitoring;

public sealed record UpdateKubernetesIngressMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);