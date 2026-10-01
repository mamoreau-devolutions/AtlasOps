namespace AtlasOps.Features.Kubernetes.KubernetesServiceProvisioning;

public sealed record UpdateKubernetesServiceProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);