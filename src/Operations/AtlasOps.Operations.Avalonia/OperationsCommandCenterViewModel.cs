namespace AtlasOps.Operations.Avalonia;

public sealed class OperationsCommandCenterViewModel : OperationsWorkbenchViewModel
{
    public OperationsCommandCenterViewModel()
        : base(
            "Operations command center",
            "Durable jobs, leases, retries, dead letters, and recovery actions.",
            CreateRows())
    {
    }

    private static IReadOnlyList<OperationRowViewModel> CreateRows()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        return
        [
            new("job-sync-git", "Synchronize provider delta", "Engineering", "Running", "Processing cursor page 18.", now, "Information"),
            new("job-reconcile-k8s", "Reconcile production cluster", "Infrastructure", "Retry scheduled", "Lease expired and was recovered.", now.AddMinutes(-3), "Warning"),
            new("job-mail-alert", "Deliver escalation digest", "Communications", "Succeeded", "Delivered to 12 recipients.", now.AddMinutes(-8), "Success"),
            new("job-identity", "Apply identity deprovisioning", "Identity", "Queued", "Waiting for the identity provider lease.", now.AddMinutes(-11), "Information"),
        ];
    }
}
