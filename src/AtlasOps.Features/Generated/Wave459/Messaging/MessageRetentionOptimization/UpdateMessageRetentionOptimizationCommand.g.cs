namespace AtlasOps.Features.Messaging.MessageRetentionOptimization;

public sealed record UpdateMessageRetentionOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);