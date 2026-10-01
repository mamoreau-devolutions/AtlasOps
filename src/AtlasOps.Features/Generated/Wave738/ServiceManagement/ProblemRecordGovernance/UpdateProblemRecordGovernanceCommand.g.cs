namespace AtlasOps.Features.ServiceManagement.ProblemRecordGovernance;

public sealed record UpdateProblemRecordGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);