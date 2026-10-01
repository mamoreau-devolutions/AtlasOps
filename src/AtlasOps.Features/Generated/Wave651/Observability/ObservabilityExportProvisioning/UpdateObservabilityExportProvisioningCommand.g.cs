namespace AtlasOps.Features.Observability.ObservabilityExportProvisioning;

public sealed record UpdateObservabilityExportProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);