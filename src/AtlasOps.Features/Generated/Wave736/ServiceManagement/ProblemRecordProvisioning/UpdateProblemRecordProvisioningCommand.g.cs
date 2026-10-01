namespace AtlasOps.Features.ServiceManagement.ProblemRecordProvisioning;

public sealed record UpdateProblemRecordProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);