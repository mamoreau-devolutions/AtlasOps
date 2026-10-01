namespace AtlasOps.Features.Messaging.MessageProducerGovernance;

public sealed record UpdateMessageProducerGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);