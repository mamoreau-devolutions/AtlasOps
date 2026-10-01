namespace AtlasOps.Features.ServiceManagement.KnowledgeArticleProvisioning;

public sealed record KnowledgeArticleProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);