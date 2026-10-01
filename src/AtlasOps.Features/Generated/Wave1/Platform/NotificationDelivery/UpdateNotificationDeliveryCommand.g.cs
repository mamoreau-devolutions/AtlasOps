namespace AtlasOps.Features.Platform.NotificationDelivery;

public sealed record UpdateNotificationDeliveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);