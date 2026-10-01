namespace AtlasOps.Features.ServiceManagement.ProblemRecordRecovery;

public sealed record ProblemRecordRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);