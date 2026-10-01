namespace AtlasOps.App.Infrastructure;

public enum AtlasOpsNotificationSeverity
{
    Information,
    Success,
    Warning,
    Error,
}

public sealed record AtlasOpsNotification(
    string Message,
    AtlasOpsNotificationSeverity Severity,
    DateTimeOffset CreatedAt);

public sealed class AtlasOpsNotificationService
{
    public event EventHandler<AtlasOpsNotification>? NotificationPublished;

    public void Publish(
        string message,
        AtlasOpsNotificationSeverity severity = AtlasOpsNotificationSeverity.Information)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("A notification message is required.", nameof(message));
        }

        AtlasOpsNotification notification = new(message, severity, DateTimeOffset.UtcNow);
        this.NotificationPublished?.Invoke(this, notification);
    }
}