namespace AtlasOps.Features.ServiceManagement.KnowledgeArticleProvisioning;

using AtlasOps.Features;

public sealed class KnowledgeArticleProvisioningService(
    IAtlasOpsCapabilityRepository<KnowledgeArticleProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly KnowledgeArticleProvisioningValidator validator = new();
    private readonly KnowledgeArticleProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<KnowledgeArticleProvisioningChanged>> ExecuteAsync(
        UpdateKnowledgeArticleProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KnowledgeArticleProvisioningChanged>.Invalid(issues);
        }

        KnowledgeArticleProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KnowledgeArticleProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KnowledgeArticleProvisioningChanged>.Invalid(
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

        KnowledgeArticleProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KnowledgeArticleProvisioningChanged>.Success(changed);
    }
}