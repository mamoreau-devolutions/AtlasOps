namespace AtlasOps.Features.Kubernetes.KubernetesPodOptimization;

public sealed record UpdateKubernetesPodOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);