namespace AtlasOps.Features.Cloud.CloudRegionProvisioning;

public sealed record CloudRegionProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);