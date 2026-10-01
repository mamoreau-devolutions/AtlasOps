namespace AtlasOps.Features.Desktop.DesktopUpdateGovernance;

public sealed record DesktopUpdateGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);