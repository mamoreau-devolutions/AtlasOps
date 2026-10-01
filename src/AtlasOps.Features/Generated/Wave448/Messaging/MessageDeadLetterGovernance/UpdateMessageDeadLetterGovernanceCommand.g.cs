namespace AtlasOps.Features.Messaging.MessageDeadLetterGovernance;

public sealed record UpdateMessageDeadLetterGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);