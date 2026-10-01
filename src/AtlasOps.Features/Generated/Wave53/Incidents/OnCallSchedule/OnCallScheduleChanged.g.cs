namespace AtlasOps.Features.Incidents.OnCallSchedule;

public sealed record OnCallScheduleChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);