namespace AtlasOps.Features.Editor.SyntaxProfile;

public sealed record UpdateSyntaxProfileCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);