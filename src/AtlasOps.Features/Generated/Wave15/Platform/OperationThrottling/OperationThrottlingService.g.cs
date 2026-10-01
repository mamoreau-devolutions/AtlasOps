namespace AtlasOps.Features.Platform.OperationThrottling;

using AtlasOps.Features;

public sealed class OperationThrottlingService(
    IAtlasOpsCapabilityRepository<OperationThrottlingItem> repository,
    TimeProvider timeProvider)
{
    private readonly OperationThrottlingValidator validator = new();
    private readonly OperationThrottlingPolicy policy = new();

    public async Task<AtlasOpsOperationResult<OperationThrottlingChanged>> ExecuteAsync(
        UpdateOperationThrottlingCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<OperationThrottlingChanged>.Invalid(issues);
        }

        OperationThrottlingItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new OperationThrottlingItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<OperationThrottlingChanged>.Invalid(
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

        OperationThrottlingChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<OperationThrottlingChanged>.Success(changed);
    }
}