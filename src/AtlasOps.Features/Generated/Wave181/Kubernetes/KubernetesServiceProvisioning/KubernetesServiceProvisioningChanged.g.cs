namespace AtlasOps.Features.Kubernetes.KubernetesServiceProvisioning;

public sealed record KubernetesServiceProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);