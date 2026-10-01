namespace AtlasOps.Features.Kubernetes.KubernetesOperatorProvisioning;

public sealed record UpdateKubernetesOperatorProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);