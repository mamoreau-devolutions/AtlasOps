namespace AtlasOps.Features.Incidents.StakeholderSubscription;

using AtlasOps.Features;

public sealed class StakeholderSubscriptionService(
    IAtlasOpsCapabilityRepository<StakeholderSubscriptionItem> repository,
    TimeProvider timeProvider)
{
    private readonly StakeholderSubscriptionValidator validator = new();
    private readonly StakeholderSubscriptionPolicy policy = new();

    public async Task<AtlasOpsOperationResult<StakeholderSubscriptionChanged>> ExecuteAsync(
        UpdateStakeholderSubscriptionCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<StakeholderSubscriptionChanged>.Invalid(issues);
        }

        StakeholderSubscriptionItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new StakeholderSubscriptionItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<StakeholderSubscriptionChanged>.Invalid(
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

        StakeholderSubscriptionChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<StakeholderSubscriptionChanged>.Success(changed);
    }
}