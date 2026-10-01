namespace AtlasOps.Features.Platform.DialogCoordination;

using AtlasOps.Features;

public sealed class DialogCoordinationService(
    IAtlasOpsCapabilityRepository<DialogCoordinationItem> repository,
    TimeProvider timeProvider)
{
    private readonly DialogCoordinationValidator validator = new();
    private readonly DialogCoordinationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DialogCoordinationChanged>> ExecuteAsync(
        UpdateDialogCoordinationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DialogCoordinationChanged>.Invalid(issues);
        }

        DialogCoordinationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DialogCoordinationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DialogCoordinationChanged>.Invalid(
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

        DialogCoordinationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DialogCoordinationChanged>.Success(changed);
    }
}