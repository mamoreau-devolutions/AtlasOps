namespace AtlasOps.Features.Cloud.CloudFunctionRecovery;

public sealed record CloudFunctionRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);