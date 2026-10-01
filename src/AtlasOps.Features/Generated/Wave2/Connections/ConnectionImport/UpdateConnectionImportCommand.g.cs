namespace AtlasOps.Features.Connections.ConnectionImport;

public sealed record UpdateConnectionImportCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);