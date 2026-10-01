namespace AtlasOps.Features.Messaging.MessageBrokerGovernance;

public sealed record UpdateMessageBrokerGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);