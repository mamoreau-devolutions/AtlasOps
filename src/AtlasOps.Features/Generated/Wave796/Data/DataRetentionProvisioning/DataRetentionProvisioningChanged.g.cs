namespace AtlasOps.Features.Data.DataRetentionProvisioning;

public sealed record DataRetentionProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);