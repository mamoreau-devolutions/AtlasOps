namespace AtlasOps.Features.Cloud.AwsAccountProvisioning;

public sealed record AwsAccountProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);