namespace AtlasOps.Features.Delivery.SourceRepositoryProvisioning;

public sealed record UpdateSourceRepositoryProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);