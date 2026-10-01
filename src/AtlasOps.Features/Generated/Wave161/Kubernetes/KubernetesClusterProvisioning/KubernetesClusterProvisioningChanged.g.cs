namespace AtlasOps.Features.Kubernetes.KubernetesClusterProvisioning;

public sealed record KubernetesClusterProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);