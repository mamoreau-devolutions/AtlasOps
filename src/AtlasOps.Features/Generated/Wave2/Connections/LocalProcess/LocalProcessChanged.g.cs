namespace AtlasOps.Features.Connections.LocalProcess;

public sealed record LocalProcessChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);