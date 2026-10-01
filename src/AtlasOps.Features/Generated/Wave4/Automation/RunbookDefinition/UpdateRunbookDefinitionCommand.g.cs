namespace AtlasOps.Features.Automation.RunbookDefinition;

public sealed record UpdateRunbookDefinitionCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);