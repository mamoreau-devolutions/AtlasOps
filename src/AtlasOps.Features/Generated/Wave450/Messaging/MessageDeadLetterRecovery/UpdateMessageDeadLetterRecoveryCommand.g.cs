namespace AtlasOps.Features.Messaging.MessageDeadLetterRecovery;

public sealed record UpdateMessageDeadLetterRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);