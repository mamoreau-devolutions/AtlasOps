namespace AtlasOps.Features.Governance.RoleDefinition;

public sealed record RoleDefinitionChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);