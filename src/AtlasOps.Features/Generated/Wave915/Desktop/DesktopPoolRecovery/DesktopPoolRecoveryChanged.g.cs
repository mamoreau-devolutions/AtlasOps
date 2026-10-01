namespace AtlasOps.Features.Desktop.DesktopPoolRecovery;

public sealed record DesktopPoolRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);