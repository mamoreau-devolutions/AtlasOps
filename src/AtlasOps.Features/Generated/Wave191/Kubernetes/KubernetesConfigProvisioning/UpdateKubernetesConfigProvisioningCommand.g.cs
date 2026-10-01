namespace AtlasOps.Features.Kubernetes.KubernetesConfigProvisioning;

public sealed record UpdateKubernetesConfigProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);