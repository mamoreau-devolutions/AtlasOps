namespace AtlasOps.Integrations.Communications.Avalonia;

using AtlasOps.Operations.Avalonia;

public sealed class CommunicationsWorkbenchViewModel : OperationsWorkbenchViewModel
{
    public CommunicationsWorkbenchViewModel()
        : base(
            "Communications integrations",
            "Message delivery, mail planning, signed webhooks, retries, and dead-letter recovery.",
            CreateRows())
    {
    }

    private static IReadOnlyList<OperationRowViewModel> CreateRows()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        return
        [
            new("message-delivery", "Deliver provider notifications", "Messaging", "Retry scheduled", "Provider throttled delivery for 45 seconds.", now, "Warning"),
            new("mail-digest", "Send operations digest", "Mail", "Ready", "Attachments and restricted headers validated.", now.AddMinutes(-3), "Success"),
            new("webhook-verify", "Verify incoming webhook signatures", "Webhooks", "Running", "Replay window and idempotency keys are being checked.", now.AddMinutes(-5), "Information"),
        ];
    }
}
