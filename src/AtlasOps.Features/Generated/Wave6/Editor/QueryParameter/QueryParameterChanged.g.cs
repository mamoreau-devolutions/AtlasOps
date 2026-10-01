namespace AtlasOps.Features.Editor.QueryParameter;

public sealed record QueryParameterChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);