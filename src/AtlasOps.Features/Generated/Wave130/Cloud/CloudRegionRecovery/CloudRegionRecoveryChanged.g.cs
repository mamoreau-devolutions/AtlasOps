namespace AtlasOps.Features.Cloud.CloudRegionRecovery;

public sealed record CloudRegionRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);