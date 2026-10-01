namespace AtlasOps.Integrations.Infrastructure.Avalonia;

using AtlasOps.Operations.Avalonia;

public sealed class InfrastructureWorkbenchViewModel : OperationsWorkbenchViewModel
{
    public InfrastructureWorkbenchViewModel()
        : base(
            "Infrastructure integrations",
            "Kubernetes reconciliation, secure remote execution, database plans, and object-storage transfers.",
            CreateRows())
    {
    }

    private static IReadOnlyList<OperationRowViewModel> CreateRows()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        return
        [
            new("k8s-reconcile", "Reconcile production namespaces", "Kubernetes", "Drift detected", "Six resources require ordered changes.", now, "Warning"),
            new("ssh-audit", "Validate remote execution plans", "Secure shell", "Ready", "Host-key policies validated for 48 endpoints.", now.AddMinutes(-3), "Success"),
            new("db-query", "Review parameterized data plans", "Databases", "Blocked", "One remote endpoint does not require TLS.", now.AddMinutes(-5), "Error"),
            new("blob-upload", "Resume multipart uploads", "Object storage", "Running", "Three transfer plans resumed from checkpoints.", now.AddMinutes(-8), "Information"),
        ];
    }
}
