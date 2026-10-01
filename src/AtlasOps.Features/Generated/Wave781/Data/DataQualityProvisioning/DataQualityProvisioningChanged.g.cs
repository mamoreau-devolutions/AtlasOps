namespace AtlasOps.Features.Data.DataQualityProvisioning;

public sealed record DataQualityProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);