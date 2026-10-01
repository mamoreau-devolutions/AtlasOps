namespace AtlasOps.Features.Cloud.AwsAccountRecovery;

public sealed record AwsAccountRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);