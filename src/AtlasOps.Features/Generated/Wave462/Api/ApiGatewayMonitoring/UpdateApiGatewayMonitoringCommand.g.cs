namespace AtlasOps.Features.Api.ApiGatewayMonitoring;

public sealed record UpdateApiGatewayMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);