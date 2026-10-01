namespace AtlasOps.Features.Kubernetes.KubernetesConfigProvisioning;

public sealed record KubernetesConfigProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);