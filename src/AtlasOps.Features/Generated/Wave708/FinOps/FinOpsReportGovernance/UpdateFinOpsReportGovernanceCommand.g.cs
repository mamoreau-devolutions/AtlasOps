namespace AtlasOps.Features.FinOps.FinOpsReportGovernance;

public sealed record UpdateFinOpsReportGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);