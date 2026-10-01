namespace AtlasOps.Features.Hardening.RollbackPackage;

public sealed record UpdateRollbackPackageCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);