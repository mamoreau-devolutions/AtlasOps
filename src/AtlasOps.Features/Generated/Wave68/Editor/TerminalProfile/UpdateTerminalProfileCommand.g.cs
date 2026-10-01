namespace AtlasOps.Features.Editor.TerminalProfile;

public sealed record UpdateTerminalProfileCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);