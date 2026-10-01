namespace AtlasOps.Features.Api.ApiContractGovernance;

public sealed record UpdateApiContractGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);