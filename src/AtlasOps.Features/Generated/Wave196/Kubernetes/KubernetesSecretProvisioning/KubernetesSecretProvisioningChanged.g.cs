namespace AtlasOps.Features.Kubernetes.KubernetesSecretProvisioning;

public sealed record KubernetesSecretProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);