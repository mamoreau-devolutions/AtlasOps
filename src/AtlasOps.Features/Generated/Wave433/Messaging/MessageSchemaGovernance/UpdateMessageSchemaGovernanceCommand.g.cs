namespace AtlasOps.Features.Messaging.MessageSchemaGovernance;

public sealed record UpdateMessageSchemaGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);