namespace AtlasOps.Features.Kubernetes.KubernetesClusterProvisioning;

public sealed record UpdateKubernetesClusterProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);