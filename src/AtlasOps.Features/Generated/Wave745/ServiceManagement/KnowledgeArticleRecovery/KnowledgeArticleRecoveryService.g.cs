namespace AtlasOps.Features.ServiceManagement.KnowledgeArticleRecovery;

using AtlasOps.Features;

public sealed class KnowledgeArticleRecoveryService(
    IAtlasOpsCapabilityRepository<KnowledgeArticleRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly KnowledgeArticleRecoveryValidator validator = new();
    private readonly KnowledgeArticleRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<KnowledgeArticleRecoveryChanged>> ExecuteAsync(
        UpdateKnowledgeArticleRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KnowledgeArticleRecoveryChanged>.Invalid(issues);
        }

        KnowledgeArticleRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KnowledgeArticleRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KnowledgeArticleRecoveryChanged>.Invalid(
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

        KnowledgeArticleRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KnowledgeArticleRecoveryChanged>.Success(changed);
    }
}