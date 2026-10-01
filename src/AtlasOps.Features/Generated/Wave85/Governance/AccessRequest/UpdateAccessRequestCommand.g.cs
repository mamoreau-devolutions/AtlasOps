namespace AtlasOps.Features.Governance.AccessRequest;

public sealed record UpdateAccessRequestCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);