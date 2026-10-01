namespace AtlasOps.Features.Database.DatabaseCredentialRecovery;

public sealed record DatabaseCredentialRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);