namespace AtlasOps.Features.Api.ApiTokenGovernance;

public sealed record UpdateApiTokenGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);