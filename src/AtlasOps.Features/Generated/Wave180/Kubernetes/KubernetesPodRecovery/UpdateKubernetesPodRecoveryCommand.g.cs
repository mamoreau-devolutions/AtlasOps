namespace AtlasOps.Features.Kubernetes.KubernetesPodRecovery;

public sealed record UpdateKubernetesPodRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);