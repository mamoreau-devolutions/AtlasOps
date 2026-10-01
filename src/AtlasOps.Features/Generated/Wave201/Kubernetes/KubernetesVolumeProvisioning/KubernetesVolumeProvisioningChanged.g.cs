namespace AtlasOps.Features.Kubernetes.KubernetesVolumeProvisioning;

public sealed record KubernetesVolumeProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);