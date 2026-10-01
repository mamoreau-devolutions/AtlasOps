namespace AtlasOps.Features.Data.DataTransformProvisioning;

public sealed record DataTransformProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);