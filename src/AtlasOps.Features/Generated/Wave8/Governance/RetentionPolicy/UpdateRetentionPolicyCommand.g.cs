namespace AtlasOps.Features.Governance.RetentionPolicy;

public sealed record UpdateRetentionPolicyCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);