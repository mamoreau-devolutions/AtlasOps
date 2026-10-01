namespace AtlasOps.Features.Cloud.CloudIdentityRecovery;

public sealed record CloudIdentityRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);