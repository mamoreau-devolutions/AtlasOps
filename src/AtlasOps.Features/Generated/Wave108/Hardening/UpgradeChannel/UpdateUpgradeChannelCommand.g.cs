namespace AtlasOps.Features.Hardening.UpgradeChannel;

public sealed record UpdateUpgradeChannelCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);