namespace AtlasOps.Features.Observability.TraceSpanProvisioning;

public sealed record UpdateTraceSpanProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);