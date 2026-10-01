namespace AtlasOps.Features.Messaging.MessageSchemaRecovery;

public sealed record UpdateMessageSchemaRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);