namespace AtlasOps.Features.Platform.LayoutPersistence;

public sealed record UpdateLayoutPersistenceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);