namespace AtlasOps.Features.Editor.DataExport;

public sealed record UpdateDataExportCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);