namespace AtlasOps.Features.Analytics.SharedFilter;

using AtlasOps.Features;

public sealed class SharedFilterService(
    IAtlasOpsCapabilityRepository<SharedFilterItem> repository,
    TimeProvider timeProvider)
{
    private readonly SharedFilterValidator validator = new();
    private readonly SharedFilterPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SharedFilterChanged>> ExecuteAsync(
        UpdateSharedFilterCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SharedFilterChanged>.Invalid(issues);
        }

        SharedFilterItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SharedFilterItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SharedFilterChanged>.Invalid(
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

        SharedFilterChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SharedFilterChanged>.Success(changed);
    }
}