namespace AtlasOps.Features.Compute.ComputeConsoleProvisioning;

public sealed record UpdateComputeConsoleProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);