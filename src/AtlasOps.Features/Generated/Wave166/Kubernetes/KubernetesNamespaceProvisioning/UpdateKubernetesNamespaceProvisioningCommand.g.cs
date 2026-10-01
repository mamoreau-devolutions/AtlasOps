namespace AtlasOps.Features.Kubernetes.KubernetesNamespaceProvisioning;

public sealed record UpdateKubernetesNamespaceProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);