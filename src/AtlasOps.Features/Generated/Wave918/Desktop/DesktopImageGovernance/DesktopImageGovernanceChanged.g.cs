namespace AtlasOps.Features.Desktop.DesktopImageGovernance;

public sealed record DesktopImageGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);