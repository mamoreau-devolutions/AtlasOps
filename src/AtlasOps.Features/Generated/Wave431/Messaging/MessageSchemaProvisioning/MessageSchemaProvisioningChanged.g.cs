namespace AtlasOps.Features.Messaging.MessageSchemaProvisioning;

public sealed record MessageSchemaProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);