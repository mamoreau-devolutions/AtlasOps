namespace AtlasOps.Features.Platform.ModuleRegistration;

public sealed record UpdateModuleRegistrationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);