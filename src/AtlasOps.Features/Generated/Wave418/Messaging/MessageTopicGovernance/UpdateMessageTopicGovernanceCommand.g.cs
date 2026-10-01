namespace AtlasOps.Features.Messaging.MessageTopicGovernance;

public sealed record UpdateMessageTopicGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);