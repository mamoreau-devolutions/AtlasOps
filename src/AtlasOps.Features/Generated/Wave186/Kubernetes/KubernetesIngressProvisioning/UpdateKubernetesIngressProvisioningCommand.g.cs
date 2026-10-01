namespace AtlasOps.Features.Kubernetes.KubernetesIngressProvisioning;

public sealed record UpdateKubernetesIngressProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);