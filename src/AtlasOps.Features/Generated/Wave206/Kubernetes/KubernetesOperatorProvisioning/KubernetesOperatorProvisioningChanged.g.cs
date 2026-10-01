namespace AtlasOps.Features.Kubernetes.KubernetesOperatorProvisioning;

public sealed record KubernetesOperatorProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);