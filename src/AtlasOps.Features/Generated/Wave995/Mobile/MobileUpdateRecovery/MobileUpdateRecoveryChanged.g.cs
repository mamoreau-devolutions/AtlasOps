namespace AtlasOps.Features.Mobile.MobileUpdateRecovery;

public sealed record MobileUpdateRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);