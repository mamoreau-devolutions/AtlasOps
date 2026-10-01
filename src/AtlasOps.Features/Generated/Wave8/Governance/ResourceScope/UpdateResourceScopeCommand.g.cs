namespace AtlasOps.Features.Governance.ResourceScope;

public sealed record UpdateResourceScopeCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);