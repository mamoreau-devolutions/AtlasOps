namespace AtlasOps.Features.Kubernetes.KubernetesWorkloadMonitoring;

public sealed record UpdateKubernetesWorkloadMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);