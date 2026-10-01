namespace AtlasOps.Features.Desktop.DesktopApplicationRecovery;

public sealed record DesktopApplicationRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);