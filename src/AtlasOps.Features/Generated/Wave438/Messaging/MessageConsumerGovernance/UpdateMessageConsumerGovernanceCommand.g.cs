namespace AtlasOps.Features.Messaging.MessageConsumerGovernance;

public sealed record UpdateMessageConsumerGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);