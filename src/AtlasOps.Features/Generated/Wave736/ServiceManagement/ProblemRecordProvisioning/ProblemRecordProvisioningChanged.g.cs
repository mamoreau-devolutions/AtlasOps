namespace AtlasOps.Features.ServiceManagement.ProblemRecordProvisioning;

public sealed record ProblemRecordProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);