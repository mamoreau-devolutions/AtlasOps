namespace AtlasOps.Features.Api.ApiContractRecovery;

public sealed record UpdateApiContractRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);