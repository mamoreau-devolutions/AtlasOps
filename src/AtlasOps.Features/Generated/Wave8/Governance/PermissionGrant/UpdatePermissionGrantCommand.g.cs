namespace AtlasOps.Features.Governance.PermissionGrant;

public sealed record UpdatePermissionGrantCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);