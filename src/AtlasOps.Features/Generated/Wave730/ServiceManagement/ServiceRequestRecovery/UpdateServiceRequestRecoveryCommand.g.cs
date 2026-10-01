namespace AtlasOps.Features.ServiceManagement.ServiceRequestRecovery;

public sealed record UpdateServiceRequestRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);