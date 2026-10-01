namespace AtlasOps.Features.Platform.ThemeComposition;

public sealed record UpdateThemeCompositionCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);