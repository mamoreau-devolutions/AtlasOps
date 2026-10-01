namespace AtlasOps.Features.Compute.ComputeTemplateRecovery;

public sealed record ComputeTemplateRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);