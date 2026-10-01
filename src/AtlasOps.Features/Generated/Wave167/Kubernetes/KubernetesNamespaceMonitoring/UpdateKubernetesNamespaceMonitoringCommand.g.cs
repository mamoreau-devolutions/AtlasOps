namespace AtlasOps.Features.Kubernetes.KubernetesNamespaceMonitoring;

public sealed record UpdateKubernetesNamespaceMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);