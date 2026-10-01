namespace AtlasOps.Features.ServiceManagement.ChangeRequestRecovery;

public sealed record UpdateChangeRequestRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);