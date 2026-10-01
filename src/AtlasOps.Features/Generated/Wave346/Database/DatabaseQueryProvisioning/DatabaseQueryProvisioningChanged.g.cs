namespace AtlasOps.Features.Database.DatabaseQueryProvisioning;

public sealed record DatabaseQueryProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);