namespace AtlasOps.Features.Messaging.MessageRetentionRecovery;

public sealed record UpdateMessageRetentionRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);