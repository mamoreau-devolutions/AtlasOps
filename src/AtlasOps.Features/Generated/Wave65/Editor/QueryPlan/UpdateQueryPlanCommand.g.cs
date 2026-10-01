namespace AtlasOps.Features.Editor.QueryPlan;

public sealed record UpdateQueryPlanCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);