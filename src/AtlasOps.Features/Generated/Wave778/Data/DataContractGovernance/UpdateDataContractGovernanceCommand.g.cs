namespace AtlasOps.Features.Data.DataContractGovernance;

public sealed record UpdateDataContractGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);