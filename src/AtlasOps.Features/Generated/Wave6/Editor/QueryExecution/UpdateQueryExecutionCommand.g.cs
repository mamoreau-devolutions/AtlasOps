namespace AtlasOps.Features.Editor.QueryExecution;

public sealed record UpdateQueryExecutionCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);