namespace AtlasOps.Features.Incidents.ResponderAssignment;

public sealed record ResponderAssignmentChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);