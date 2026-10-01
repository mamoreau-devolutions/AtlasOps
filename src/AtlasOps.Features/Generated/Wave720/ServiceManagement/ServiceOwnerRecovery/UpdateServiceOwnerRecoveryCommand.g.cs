namespace AtlasOps.Features.ServiceManagement.ServiceOwnerRecovery;

public sealed record UpdateServiceOwnerRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);