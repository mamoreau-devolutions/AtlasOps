namespace AtlasOps.Features.Automation.RunbookDefinition;

public sealed record RunbookDefinitionChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);