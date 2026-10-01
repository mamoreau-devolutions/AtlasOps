namespace AtlasOps.Features.Compute.ComputeScheduleRecovery;

using AtlasOps.Features;

public sealed class ComputeScheduleRecoveryService(
    IAtlasOpsCapabilityRepository<ComputeScheduleRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly ComputeScheduleRecoveryValidator validator = new();
    private readonly ComputeScheduleRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ComputeScheduleRecoveryChanged>> ExecuteAsync(
        UpdateComputeScheduleRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ComputeScheduleRecoveryChanged>.Invalid(issues);
        }

        ComputeScheduleRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ComputeScheduleRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ComputeScheduleRecoveryChanged>.Invalid(
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

        ComputeScheduleRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ComputeScheduleRecoveryChanged>.Success(changed);
    }
}