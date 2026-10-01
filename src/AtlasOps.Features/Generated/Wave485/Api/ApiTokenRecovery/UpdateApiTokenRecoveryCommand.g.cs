namespace AtlasOps.Features.Api.ApiTokenRecovery;

public sealed record UpdateApiTokenRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);