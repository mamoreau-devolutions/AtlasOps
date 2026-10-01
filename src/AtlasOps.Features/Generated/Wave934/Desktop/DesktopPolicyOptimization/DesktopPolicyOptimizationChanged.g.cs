namespace AtlasOps.Features.Desktop.DesktopPolicyOptimization;

public sealed record DesktopPolicyOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);