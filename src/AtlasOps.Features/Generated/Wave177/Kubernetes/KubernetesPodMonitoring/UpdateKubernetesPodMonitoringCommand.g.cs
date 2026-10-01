namespace AtlasOps.Features.Kubernetes.KubernetesPodMonitoring;

public sealed record UpdateKubernetesPodMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);