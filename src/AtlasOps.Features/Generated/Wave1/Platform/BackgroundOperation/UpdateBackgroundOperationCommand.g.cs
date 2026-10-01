namespace AtlasOps.Features.Platform.BackgroundOperation;

public sealed record UpdateBackgroundOperationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);