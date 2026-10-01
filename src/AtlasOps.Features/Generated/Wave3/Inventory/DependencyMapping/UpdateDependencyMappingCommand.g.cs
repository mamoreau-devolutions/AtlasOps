namespace AtlasOps.Features.Inventory.DependencyMapping;

public sealed record UpdateDependencyMappingCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);