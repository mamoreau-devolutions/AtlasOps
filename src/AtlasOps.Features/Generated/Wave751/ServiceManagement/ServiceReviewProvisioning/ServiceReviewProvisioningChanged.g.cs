namespace AtlasOps.Features.ServiceManagement.ServiceReviewProvisioning;

public sealed record ServiceReviewProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);