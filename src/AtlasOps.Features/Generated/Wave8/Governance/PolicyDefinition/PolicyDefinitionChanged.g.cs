namespace AtlasOps.Features.Governance.PolicyDefinition;

public sealed record PolicyDefinitionChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);