namespace AtlasOps.Features.ServiceManagement.ProblemRecordOptimization;

public sealed record UpdateProblemRecordOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);