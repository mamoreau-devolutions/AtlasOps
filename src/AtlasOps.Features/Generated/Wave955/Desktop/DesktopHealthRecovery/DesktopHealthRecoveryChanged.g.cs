namespace AtlasOps.Features.Desktop.DesktopHealthRecovery;

public sealed record DesktopHealthRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);