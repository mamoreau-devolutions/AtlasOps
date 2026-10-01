namespace AtlasOps.Features.Desktop.DesktopSessionGovernance;

public sealed record DesktopSessionGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);