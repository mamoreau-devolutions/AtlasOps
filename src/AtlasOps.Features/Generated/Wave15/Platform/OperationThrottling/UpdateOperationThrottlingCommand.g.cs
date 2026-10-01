namespace AtlasOps.Features.Platform.OperationThrottling;

public sealed record UpdateOperationThrottlingCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);