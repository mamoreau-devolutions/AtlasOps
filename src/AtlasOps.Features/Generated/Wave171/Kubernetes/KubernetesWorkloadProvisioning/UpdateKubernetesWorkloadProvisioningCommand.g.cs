namespace AtlasOps.Features.Kubernetes.KubernetesWorkloadProvisioning;

public sealed record UpdateKubernetesWorkloadProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);