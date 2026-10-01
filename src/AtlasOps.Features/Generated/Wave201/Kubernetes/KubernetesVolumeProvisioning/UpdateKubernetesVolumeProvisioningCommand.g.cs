namespace AtlasOps.Features.Kubernetes.KubernetesVolumeProvisioning;

public sealed record UpdateKubernetesVolumeProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);