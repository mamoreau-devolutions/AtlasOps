namespace AtlasOps.Features.Api.ApiDeploymentGovernance;

public sealed record UpdateApiDeploymentGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);