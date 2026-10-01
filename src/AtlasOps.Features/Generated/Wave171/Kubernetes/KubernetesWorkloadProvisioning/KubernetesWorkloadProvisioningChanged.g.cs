namespace AtlasOps.Features.Kubernetes.KubernetesWorkloadProvisioning;

public sealed record KubernetesWorkloadProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);