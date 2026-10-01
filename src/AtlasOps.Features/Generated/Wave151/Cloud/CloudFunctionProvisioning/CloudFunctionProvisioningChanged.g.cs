namespace AtlasOps.Features.Cloud.CloudFunctionProvisioning;

public sealed record CloudFunctionProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);