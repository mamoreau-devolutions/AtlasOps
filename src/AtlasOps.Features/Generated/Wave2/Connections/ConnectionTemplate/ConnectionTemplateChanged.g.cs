namespace AtlasOps.Features.Connections.ConnectionTemplate;

public sealed record ConnectionTemplateChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);