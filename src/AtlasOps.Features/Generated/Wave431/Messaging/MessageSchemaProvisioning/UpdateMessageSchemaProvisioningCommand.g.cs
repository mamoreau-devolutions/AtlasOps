namespace AtlasOps.Features.Messaging.MessageSchemaProvisioning;

public sealed record UpdateMessageSchemaProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);