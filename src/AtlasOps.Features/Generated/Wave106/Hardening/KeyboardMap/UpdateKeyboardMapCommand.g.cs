namespace AtlasOps.Features.Hardening.KeyboardMap;

public sealed record UpdateKeyboardMapCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);