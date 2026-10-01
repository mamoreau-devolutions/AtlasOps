namespace AtlasOps.Features.Editor.QueryDefinition;

public sealed record UpdateQueryDefinitionCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);