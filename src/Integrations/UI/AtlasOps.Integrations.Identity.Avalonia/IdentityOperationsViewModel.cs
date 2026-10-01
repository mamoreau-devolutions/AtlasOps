namespace AtlasOps.Integrations.Identity.Avalonia;

using AtlasOps.Operations.Avalonia;

public sealed class IdentityOperationsViewModel : OperationsWorkbenchViewModel
{
    public IdentityOperationsViewModel()
        : base(
            "Identity and service operations",
            "Access reconciliation, monitoring correlation, service-level clocks, and escalations.",
            CreateRows())
    {
    }

    private static IReadOnlyList<OperationRowViewModel> CreateRows()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        return
        [
            new("identity-revoke", "Apply deprovisioning plans", "Identity", "Running", "Revocations are ordered before grants.", now, "Information"),
            new("alert-correlate", "Correlate infrastructure alerts", "Monitoring", "Ready", "Grouped 82 signals into 11 incidents.", now.AddMinutes(-4), "Success"),
            new("sla-evaluate", "Evaluate service-level obligations", "Service management", "Escalated", "Three tickets exceeded resolution targets.", now.AddMinutes(-6), "Error"),
        ];
    }
}
