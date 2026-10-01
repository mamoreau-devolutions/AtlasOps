namespace AtlasOps.Features.Database.DatabaseIndexProvisioning;

public sealed record DatabaseIndexProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);