namespace AtlasOps.Features.Cloud.AzureSubscriptionRecovery;

public sealed record AzureSubscriptionRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);