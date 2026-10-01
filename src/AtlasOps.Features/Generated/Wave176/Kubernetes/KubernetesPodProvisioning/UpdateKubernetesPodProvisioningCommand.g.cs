namespace AtlasOps.Features.Kubernetes.KubernetesPodProvisioning;

public sealed record UpdateKubernetesPodProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);