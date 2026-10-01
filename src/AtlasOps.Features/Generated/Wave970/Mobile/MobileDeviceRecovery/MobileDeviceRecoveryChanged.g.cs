namespace AtlasOps.Features.Mobile.MobileDeviceRecovery;

public sealed record MobileDeviceRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);