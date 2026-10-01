namespace AtlasOps.Features.Api.ApiHealthGovernance;

public sealed record UpdateApiHealthGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);