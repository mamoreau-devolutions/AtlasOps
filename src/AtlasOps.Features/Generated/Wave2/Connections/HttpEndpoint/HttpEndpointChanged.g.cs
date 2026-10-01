namespace AtlasOps.Features.Connections.HttpEndpoint;

public sealed record HttpEndpointChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);