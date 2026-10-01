namespace AtlasOps.Features.Messaging.MessageQueueRecovery;

public sealed record UpdateMessageQueueRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);