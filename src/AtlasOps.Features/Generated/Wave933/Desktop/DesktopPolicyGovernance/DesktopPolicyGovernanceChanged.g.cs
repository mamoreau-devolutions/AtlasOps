namespace AtlasOps.Features.Desktop.DesktopPolicyGovernance;

public sealed record DesktopPolicyGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);