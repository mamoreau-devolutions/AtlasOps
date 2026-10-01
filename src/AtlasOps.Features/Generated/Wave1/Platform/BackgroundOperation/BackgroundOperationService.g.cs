namespace AtlasOps.Features.Platform.BackgroundOperation;

using AtlasOps.Features;

public sealed class BackgroundOperationService(
    IAtlasOpsCapabilityRepository<BackgroundOperationItem> repository,
    TimeProvider timeProvider)
{
    private readonly BackgroundOperationValidator validator = new();
    private readonly BackgroundOperationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<BackgroundOperationChanged>> ExecuteAsync(
        UpdateBackgroundOperationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<BackgroundOperationChanged>.Invalid(issues);
        }

        BackgroundOperationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new BackgroundOperationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<BackgroundOperationChanged>.Invalid(
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

        BackgroundOperationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<BackgroundOperationChanged>.Success(changed);
    }
}