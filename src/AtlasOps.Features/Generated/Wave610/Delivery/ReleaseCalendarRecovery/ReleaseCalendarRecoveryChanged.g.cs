namespace AtlasOps.Features.Delivery.ReleaseCalendarRecovery;

public sealed record ReleaseCalendarRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);