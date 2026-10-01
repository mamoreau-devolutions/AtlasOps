namespace AtlasOps.Features.Analytics.AnalyticsSubscription;

using AtlasOps.Features;

public sealed class AnalyticsSubscriptionService(
    IAtlasOpsCapabilityRepository<AnalyticsSubscriptionItem> repository,
    TimeProvider timeProvider)
{
    private readonly AnalyticsSubscriptionValidator validator = new();
    private readonly AnalyticsSubscriptionPolicy policy = new();

    public async Task<AtlasOpsOperationResult<AnalyticsSubscriptionChanged>> ExecuteAsync(
        UpdateAnalyticsSubscriptionCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<AnalyticsSubscriptionChanged>.Invalid(issues);
        }

        AnalyticsSubscriptionItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new AnalyticsSubscriptionItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<AnalyticsSubscriptionChanged>.Invalid(
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

        AnalyticsSubscriptionChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<AnalyticsSubscriptionChanged>.Success(changed);
    }
}