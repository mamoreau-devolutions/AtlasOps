namespace AtlasOps.Features.Sync.DurableInbox;

public sealed record DurableInboxChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);