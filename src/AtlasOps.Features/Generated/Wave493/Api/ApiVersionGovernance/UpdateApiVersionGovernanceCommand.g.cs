namespace AtlasOps.Features.Api.ApiVersionGovernance;

public sealed record UpdateApiVersionGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);