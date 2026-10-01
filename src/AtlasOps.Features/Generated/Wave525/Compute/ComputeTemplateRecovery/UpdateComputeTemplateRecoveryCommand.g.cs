namespace AtlasOps.Features.Compute.ComputeTemplateRecovery;

public sealed record UpdateComputeTemplateRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);