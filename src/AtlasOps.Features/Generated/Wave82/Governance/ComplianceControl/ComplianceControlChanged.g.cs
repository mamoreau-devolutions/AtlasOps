namespace AtlasOps.Features.Governance.ComplianceControl;

public sealed record ComplianceControlChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);