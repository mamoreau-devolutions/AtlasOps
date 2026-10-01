namespace AtlasOps.Features.ServiceManagement.KnowledgeArticleRecovery;

public sealed record KnowledgeArticleRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);