namespace AtlasOps.Features.Observability.TraceSourceProvisioning;

public sealed record UpdateTraceSourceProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);