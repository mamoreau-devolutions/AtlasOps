namespace AtlasOps.Features.ServiceManagement.KnowledgeArticleGovernance;

public sealed record UpdateKnowledgeArticleGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);