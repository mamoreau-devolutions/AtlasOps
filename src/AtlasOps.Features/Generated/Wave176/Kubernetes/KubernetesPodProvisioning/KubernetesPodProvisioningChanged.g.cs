namespace AtlasOps.Features.Kubernetes.KubernetesPodProvisioning;

public sealed record KubernetesPodProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);