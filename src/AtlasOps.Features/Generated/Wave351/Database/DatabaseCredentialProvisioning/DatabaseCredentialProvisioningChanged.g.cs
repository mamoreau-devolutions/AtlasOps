namespace AtlasOps.Features.Database.DatabaseCredentialProvisioning;

public sealed record DatabaseCredentialProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);