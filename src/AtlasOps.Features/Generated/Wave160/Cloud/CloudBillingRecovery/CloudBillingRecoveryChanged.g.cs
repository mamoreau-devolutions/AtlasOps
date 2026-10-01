namespace AtlasOps.Features.Cloud.CloudBillingRecovery;

public sealed record CloudBillingRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);