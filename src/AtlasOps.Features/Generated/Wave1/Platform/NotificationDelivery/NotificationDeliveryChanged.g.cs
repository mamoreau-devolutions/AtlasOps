namespace AtlasOps.Features.Platform.NotificationDelivery;

public sealed record NotificationDeliveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);