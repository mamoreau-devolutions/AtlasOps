namespace AtlasOps.Features.Desktop.DesktopProfileGovernance;

public sealed record DesktopProfileGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);