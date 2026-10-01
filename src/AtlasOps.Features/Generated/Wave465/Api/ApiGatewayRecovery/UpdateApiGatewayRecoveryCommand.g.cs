namespace AtlasOps.Features.Api.ApiGatewayRecovery;

public sealed record UpdateApiGatewayRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);