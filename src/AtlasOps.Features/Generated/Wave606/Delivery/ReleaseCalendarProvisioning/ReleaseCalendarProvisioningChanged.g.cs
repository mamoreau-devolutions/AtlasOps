namespace AtlasOps.Features.Delivery.ReleaseCalendarProvisioning;

public sealed record ReleaseCalendarProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);