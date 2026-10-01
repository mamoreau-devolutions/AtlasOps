namespace AtlasOps.Features.Governance.DataClassification;

public sealed record DataClassificationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);