namespace AtlasOps.Features.Compute.ComputeConsoleProvisioning;

public sealed record ComputeConsoleProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);