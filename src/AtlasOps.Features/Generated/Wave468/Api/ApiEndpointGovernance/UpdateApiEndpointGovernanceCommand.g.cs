namespace AtlasOps.Features.Api.ApiEndpointGovernance;

public sealed record UpdateApiEndpointGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);