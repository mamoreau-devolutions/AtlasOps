namespace AtlasOps.Features.Governance.ReviewCampaign;

using AtlasOps.Features;

public sealed class ReviewCampaignService(
    IAtlasOpsCapabilityRepository<ReviewCampaignItem> repository,
    TimeProvider timeProvider)
{
    private readonly ReviewCampaignValidator validator = new();
    private readonly ReviewCampaignPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ReviewCampaignChanged>> ExecuteAsync(
        UpdateReviewCampaignCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ReviewCampaignChanged>.Invalid(issues);
        }

        ReviewCampaignItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ReviewCampaignItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ReviewCampaignChanged>.Invalid(
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

        ReviewCampaignChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ReviewCampaignChanged>.Success(changed);
    }
}