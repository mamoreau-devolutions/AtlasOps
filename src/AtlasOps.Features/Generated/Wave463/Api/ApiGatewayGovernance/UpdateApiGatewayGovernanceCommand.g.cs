namespace AtlasOps.Features.Api.ApiGatewayGovernance;

public sealed record UpdateApiGatewayGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);