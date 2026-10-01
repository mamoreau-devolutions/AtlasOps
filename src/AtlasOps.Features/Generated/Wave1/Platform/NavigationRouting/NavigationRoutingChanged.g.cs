namespace AtlasOps.Features.Platform.NavigationRouting;

public sealed record NavigationRoutingChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);