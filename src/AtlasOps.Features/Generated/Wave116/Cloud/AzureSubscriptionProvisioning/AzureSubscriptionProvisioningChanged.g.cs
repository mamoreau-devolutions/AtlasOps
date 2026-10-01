namespace AtlasOps.Features.Cloud.AzureSubscriptionProvisioning;

public sealed record AzureSubscriptionProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);