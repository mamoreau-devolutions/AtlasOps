namespace AtlasOps.Features.Desktop.DesktopProfileRecovery;

public sealed record DesktopProfileRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);