namespace AtlasOps.Features.Platform.ModuleIsolation;

public sealed record UpdateModuleIsolationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);