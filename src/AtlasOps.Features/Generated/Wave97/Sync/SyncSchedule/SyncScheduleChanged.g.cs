namespace AtlasOps.Features.Sync.SyncSchedule;

public sealed record SyncScheduleChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);