namespace AtlasOps.Features.Governance.ComplianceControl;

public sealed record UpdateComplianceControlCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);