namespace AtlasOps.Features.Automation.ArtifactPromotion;

using AtlasOps.Features;

public sealed class ArtifactPromotionService(
    IAtlasOpsCapabilityRepository<ArtifactPromotionItem> repository,
    TimeProvider timeProvider)
{
    private readonly ArtifactPromotionValidator validator = new();
    private readonly ArtifactPromotionPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ArtifactPromotionChanged>> ExecuteAsync(
        UpdateArtifactPromotionCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ArtifactPromotionChanged>.Invalid(issues);
        }

        ArtifactPromotionItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ArtifactPromotionItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ArtifactPromotionChanged>.Invalid(
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

        ArtifactPromotionChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ArtifactPromotionChanged>.Success(changed);
    }
}