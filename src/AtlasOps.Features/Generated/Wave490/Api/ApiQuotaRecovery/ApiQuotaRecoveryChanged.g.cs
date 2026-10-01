namespace AtlasOps.Features.Api.ApiQuotaRecovery;

public sealed record ApiQuotaRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);