namespace AtlasOps.Features.Sync.DurableOutbox;

public sealed record DurableOutboxChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);