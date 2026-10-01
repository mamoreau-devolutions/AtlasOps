namespace AtlasOps.Features.Api.ApiAnalyticsRecovery;

public sealed record ApiAnalyticsRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);