namespace AtlasOps.Features.Kubernetes.KubernetesSecretProvisioning;

public sealed record UpdateKubernetesSecretProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);