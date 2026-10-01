namespace AtlasOps.Features.Delivery.ReleaseApprovalProvisioning;

public sealed record ReleaseApprovalProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);