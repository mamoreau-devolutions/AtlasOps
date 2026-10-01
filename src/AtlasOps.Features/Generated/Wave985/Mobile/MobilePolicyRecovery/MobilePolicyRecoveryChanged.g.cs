namespace AtlasOps.Features.Mobile.MobilePolicyRecovery;

public sealed record MobilePolicyRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);