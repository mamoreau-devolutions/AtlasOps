namespace AtlasOps.Features.Messaging.MessageQueueGovernance;

public sealed record UpdateMessageQueueGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);