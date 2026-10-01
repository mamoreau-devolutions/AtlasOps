namespace AtlasOps.Features.ServiceManagement.KnowledgeArticleGovernance;

using AtlasOps.Features;

public sealed class KnowledgeArticleGovernanceService(
    IAtlasOpsCapabilityRepository<KnowledgeArticleGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly KnowledgeArticleGovernanceValidator validator = new();
    private readonly KnowledgeArticleGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<KnowledgeArticleGovernanceChanged>> ExecuteAsync(
        UpdateKnowledgeArticleGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KnowledgeArticleGovernanceChanged>.Invalid(issues);
        }

        KnowledgeArticleGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KnowledgeArticleGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KnowledgeArticleGovernanceChanged>.Invalid(
            [
                new("State", $"Cannot transition from '{previousState}' to '{command.TargetState}'."),
            ]);
        }

        entity.Name = command.Name.Trim();
        entity.Owner = command.Owner.Trim();
        entity.State = command.TargetState;
        entity.Priority = command.Priority;
        entity.IsEnabled = command.IsEnabled;
        DateTimeOffset now = timeProvider.GetUtcNow();
        entity.MarkUpdated(now);
        await repository.SaveAsync(entity, cancellationToken);

        KnowledgeArticleGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KnowledgeArticleGovernanceChanged>.Success(changed);
    }
}