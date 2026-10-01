namespace AtlasOps.Features.ServiceManagement.KnowledgeArticleOptimization;

public sealed record KnowledgeArticleOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);