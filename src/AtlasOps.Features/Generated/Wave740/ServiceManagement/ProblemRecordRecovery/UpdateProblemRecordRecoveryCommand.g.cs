namespace AtlasOps.Features.ServiceManagement.ProblemRecordRecovery;

public sealed record UpdateProblemRecordRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);