namespace AtlasOps.Features.Editor.TerminalSession;

public sealed record UpdateTerminalSessionCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);