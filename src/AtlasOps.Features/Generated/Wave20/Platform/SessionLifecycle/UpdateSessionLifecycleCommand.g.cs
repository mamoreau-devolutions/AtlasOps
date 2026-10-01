namespace AtlasOps.Features.Platform.SessionLifecycle;

public sealed record UpdateSessionLifecycleCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);