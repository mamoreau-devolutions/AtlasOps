namespace AtlasOps.Features.Cloud.CloudFunctionOptimization;

public sealed record CloudFunctionOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);