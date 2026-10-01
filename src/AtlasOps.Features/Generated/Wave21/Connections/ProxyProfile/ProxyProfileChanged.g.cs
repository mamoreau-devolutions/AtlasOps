namespace AtlasOps.Features.Connections.ProxyProfile;

public sealed record ProxyProfileChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);