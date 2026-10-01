namespace AtlasOps.Features.Mobile.MobileTelemetryProvisioning;

public sealed record MobileTelemetryProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);