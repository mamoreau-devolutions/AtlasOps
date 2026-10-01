namespace AtlasOps.Features.Desktop.DesktopApplicationGovernance;

public sealed record DesktopApplicationGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);