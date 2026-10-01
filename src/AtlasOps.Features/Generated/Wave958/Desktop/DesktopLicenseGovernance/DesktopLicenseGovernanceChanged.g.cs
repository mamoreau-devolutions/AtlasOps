namespace AtlasOps.Features.Desktop.DesktopLicenseGovernance;

public sealed record DesktopLicenseGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);