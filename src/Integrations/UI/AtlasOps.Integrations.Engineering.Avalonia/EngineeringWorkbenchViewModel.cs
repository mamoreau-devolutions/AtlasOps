namespace AtlasOps.Integrations.Engineering.Avalonia;

using AtlasOps.Operations.Avalonia;

public sealed class EngineeringWorkbenchViewModel : OperationsWorkbenchViewModel
{
    public EngineeringWorkbenchViewModel()
        : base(
            "Engineering integrations",
            "Source-control policies, pull-request deltas, work-item transitions, and revision conflicts.",
            CreateRows())
    {
    }

    private static IReadOnlyList<OperationRowViewModel> CreateRows()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        return
        [
            new("git-policy", "Evaluate protected branch policies", "Git providers", "Ready", "12 repositories are awaiting evaluation.", now, "Information"),
            new("git-delta", "Import pull-request deltas", "Git providers", "Running", "Cursor page 18 of an incremental synchronization.", now.AddMinutes(-2), "Information"),
            new("work-transition", "Apply approved transitions", "Work tracking", "Blocked", "Two items have stale provider revisions.", now.AddMinutes(-5), "Warning"),
        ];
    }
}
