namespace AtlasOps.Features.Hardening.SupportBundle;

public sealed record UpdateSupportBundleCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);