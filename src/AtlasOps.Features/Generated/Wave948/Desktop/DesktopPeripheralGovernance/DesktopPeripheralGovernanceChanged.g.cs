namespace AtlasOps.Features.Desktop.DesktopPeripheralGovernance;

public sealed record DesktopPeripheralGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);