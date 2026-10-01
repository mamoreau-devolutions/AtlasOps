namespace AtlasOps.Features.Api.ApiEndpointRecovery;

public sealed record UpdateApiEndpointRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);