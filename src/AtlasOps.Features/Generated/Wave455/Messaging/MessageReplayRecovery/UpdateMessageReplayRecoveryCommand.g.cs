namespace AtlasOps.Features.Messaging.MessageReplayRecovery;

public sealed record UpdateMessageReplayRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);