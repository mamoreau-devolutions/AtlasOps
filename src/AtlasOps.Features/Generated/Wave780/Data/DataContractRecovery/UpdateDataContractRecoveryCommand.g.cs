namespace AtlasOps.Features.Data.DataContractRecovery;

public sealed record UpdateDataContractRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);