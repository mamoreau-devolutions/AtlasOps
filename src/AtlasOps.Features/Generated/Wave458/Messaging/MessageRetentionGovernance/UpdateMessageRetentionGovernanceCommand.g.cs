namespace AtlasOps.Features.Messaging.MessageRetentionGovernance;

public sealed record UpdateMessageRetentionGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);