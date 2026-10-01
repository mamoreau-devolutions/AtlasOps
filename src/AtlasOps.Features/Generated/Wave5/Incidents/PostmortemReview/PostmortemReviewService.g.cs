namespace AtlasOps.Features.Incidents.PostmortemReview;

using AtlasOps.Features;

public sealed class PostmortemReviewService(
    IAtlasOpsCapabilityRepository<PostmortemReviewItem> repository,
    TimeProvider timeProvider)
{
    private readonly PostmortemReviewValidator validator = new();
    private readonly PostmortemReviewPolicy policy = new();

    public async Task<AtlasOpsOperationResult<PostmortemReviewChanged>> ExecuteAsync(
        UpdatePostmortemReviewCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<PostmortemReviewChanged>.Invalid(issues);
        }

        PostmortemReviewItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new PostmortemReviewItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<PostmortemReviewChanged>.Invalid(
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

        PostmortemReviewChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<PostmortemReviewChanged>.Success(changed);
    }
}