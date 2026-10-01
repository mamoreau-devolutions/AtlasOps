namespace AtlasOps.Features.Editor.ScriptLibrary;

public sealed record UpdateScriptLibraryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);