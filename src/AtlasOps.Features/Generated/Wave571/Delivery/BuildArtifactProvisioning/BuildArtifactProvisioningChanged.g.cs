namespace AtlasOps.Features.Delivery.BuildArtifactProvisioning;

public sealed record BuildArtifactProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);