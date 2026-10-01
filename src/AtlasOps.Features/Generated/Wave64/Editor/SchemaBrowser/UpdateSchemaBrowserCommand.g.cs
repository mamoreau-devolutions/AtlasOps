namespace AtlasOps.Features.Editor.SchemaBrowser;

public sealed record UpdateSchemaBrowserCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);