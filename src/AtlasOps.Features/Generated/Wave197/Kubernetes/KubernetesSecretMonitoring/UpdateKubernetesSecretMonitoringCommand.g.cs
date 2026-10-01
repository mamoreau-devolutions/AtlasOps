namespace AtlasOps.Features.Kubernetes.KubernetesSecretMonitoring;

public sealed record UpdateKubernetesSecretMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);