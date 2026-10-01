namespace AtlasOps.Features.Kubernetes.KubernetesNamespaceProvisioning;

public sealed record KubernetesNamespaceProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);