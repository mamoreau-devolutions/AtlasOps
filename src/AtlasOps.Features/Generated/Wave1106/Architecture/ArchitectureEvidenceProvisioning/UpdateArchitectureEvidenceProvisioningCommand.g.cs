namespace AtlasOps.Features.Architecture.ArchitectureEvidenceProvisioning;

public sealed record UpdateArchitectureEvidenceProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);