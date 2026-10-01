namespace AtlasOps.Features.Observability.ObservabilitySloProvisioning;

public sealed record UpdateObservabilitySloProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);