namespace AtlasOps.Features.Desktop.DesktopHealthGovernance;

public sealed record DesktopHealthGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);