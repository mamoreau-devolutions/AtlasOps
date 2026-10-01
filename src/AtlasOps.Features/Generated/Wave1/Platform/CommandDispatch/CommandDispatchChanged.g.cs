namespace AtlasOps.Features.Platform.CommandDispatch;

public sealed record CommandDispatchChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);