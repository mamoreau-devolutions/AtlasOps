namespace AtlasOps.Features.Desktop.DesktopPoolGovernance;

public sealed record DesktopPoolGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);