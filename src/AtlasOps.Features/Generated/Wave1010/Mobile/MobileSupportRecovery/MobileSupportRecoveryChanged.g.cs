namespace AtlasOps.Features.Mobile.MobileSupportRecovery;

public sealed record MobileSupportRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);