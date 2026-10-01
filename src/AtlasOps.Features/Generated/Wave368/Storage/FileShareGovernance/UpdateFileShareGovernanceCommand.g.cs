namespace AtlasOps.Features.Storage.FileShareGovernance;

public sealed record UpdateFileShareGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);