namespace AtlasOps.Features.Analytics.SharedFilter;

public sealed record SharedFilterChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);