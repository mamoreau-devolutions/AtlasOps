namespace AtlasOps.Features.Kubernetes.KubernetesIngressProvisioning;

public sealed record KubernetesIngressProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);