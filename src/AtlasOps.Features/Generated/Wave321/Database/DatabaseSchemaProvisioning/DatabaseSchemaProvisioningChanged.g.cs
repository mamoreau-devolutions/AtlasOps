namespace AtlasOps.Features.Database.DatabaseSchemaProvisioning;

public sealed record DatabaseSchemaProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);