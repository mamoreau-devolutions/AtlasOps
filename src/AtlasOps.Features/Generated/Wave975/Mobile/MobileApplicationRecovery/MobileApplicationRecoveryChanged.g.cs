namespace AtlasOps.Features.Mobile.MobileApplicationRecovery;

public sealed record MobileApplicationRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);