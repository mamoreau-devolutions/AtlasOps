namespace AtlasOps.Features.Hardening.AccessibilityProfile;

public sealed record UpdateAccessibilityProfileCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);