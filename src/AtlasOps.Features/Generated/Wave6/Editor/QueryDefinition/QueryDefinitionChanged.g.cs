namespace AtlasOps.Features.Editor.QueryDefinition;

public sealed record QueryDefinitionChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);