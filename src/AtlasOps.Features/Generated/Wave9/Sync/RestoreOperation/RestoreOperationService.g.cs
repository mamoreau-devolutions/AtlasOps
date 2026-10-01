namespace AtlasOps.Features.Sync.RestoreOperation;

using AtlasOps.Features;

public sealed class RestoreOperationService(
    IAtlasOpsCapabilityRepository<RestoreOperationItem> repository,
    TimeProvider timeProvider)
{
    private readonly RestoreOperationValidator validator = new();
    private readonly RestoreOperationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<RestoreOperationChanged>> ExecuteAsync(
        UpdateRestoreOperationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RestoreOperationChanged>.Invalid(issues);
        }

        RestoreOperationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RestoreOperationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RestoreOperationChanged>.Invalid(
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

        RestoreOperationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RestoreOperationChanged>.Success(changed);
    }
}