namespace AtlasOps.Features.Incidents.StatusUpdate;

using AtlasOps.Features;

public sealed class StatusUpdateService(
    IAtlasOpsCapabilityRepository<StatusUpdateItem> repository,
    TimeProvider timeProvider)
{
    private readonly StatusUpdateValidator validator = new();
    private readonly StatusUpdatePolicy policy = new();

    public async Task<AtlasOpsOperationResult<StatusUpdateChanged>> ExecuteAsync(
        UpdateStatusUpdateCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<StatusUpdateChanged>.Invalid(issues);
        }

        StatusUpdateItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new StatusUpdateItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<StatusUpdateChanged>.Invalid(
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

        StatusUpdateChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<StatusUpdateChanged>.Success(changed);
    }
}