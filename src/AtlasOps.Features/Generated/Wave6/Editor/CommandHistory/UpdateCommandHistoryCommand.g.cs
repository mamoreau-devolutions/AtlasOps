namespace AtlasOps.Features.Editor.CommandHistory;

public sealed record UpdateCommandHistoryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);