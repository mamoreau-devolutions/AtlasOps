namespace AtlasOps.Features.Mobile.MobileFleetRecovery;

public sealed record MobileFleetRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);