namespace AtlasOps.Features.ServiceManagement.KnowledgeArticleGovernance;

public sealed record KnowledgeArticleGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);