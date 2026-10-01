namespace AtlasOps.Features.Api.ApiClientGovernance;

public sealed record UpdateApiClientGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);