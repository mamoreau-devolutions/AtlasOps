namespace AtlasOps.Features.Observability.ObservabilityRetentionProvisioning;

public sealed record UpdateObservabilityRetentionProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);