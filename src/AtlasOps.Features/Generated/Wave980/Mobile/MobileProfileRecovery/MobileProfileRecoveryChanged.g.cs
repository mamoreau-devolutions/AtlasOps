namespace AtlasOps.Features.Mobile.MobileProfileRecovery;

public sealed record MobileProfileRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);