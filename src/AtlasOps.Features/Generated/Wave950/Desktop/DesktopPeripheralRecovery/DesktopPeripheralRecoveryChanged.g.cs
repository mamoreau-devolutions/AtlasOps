namespace AtlasOps.Features.Desktop.DesktopPeripheralRecovery;

public sealed record DesktopPeripheralRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);