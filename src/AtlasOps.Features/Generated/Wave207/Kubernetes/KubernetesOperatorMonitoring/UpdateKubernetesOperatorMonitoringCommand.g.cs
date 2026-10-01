namespace AtlasOps.Features.Kubernetes.KubernetesOperatorMonitoring;

public sealed record UpdateKubernetesOperatorMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);