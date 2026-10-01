namespace AtlasOps.Features.Messaging.MessageReplayGovernance;

public sealed record UpdateMessageReplayGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);