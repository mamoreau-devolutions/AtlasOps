namespace AtlasOps.Features.Governance.SegregationOfDuties;

public sealed record SegregationOfDutiesChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);