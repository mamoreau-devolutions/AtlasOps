namespace AtlasOps.Features.Cloud.CloudBillingProvisioning;

public sealed record CloudBillingProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);