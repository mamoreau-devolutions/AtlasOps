namespace AtlasOps.Features.Data.DataAccessRecovery;

using AtlasOps.Features;

public sealed class DataAccessRecoveryService(
    IAtlasOpsCapabilityRepository<DataAccessRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly DataAccessRecoveryValidator validator = new();
    private readonly DataAccessRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DataAccessRecoveryChanged>> ExecuteAsync(
        UpdateDataAccessRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DataAccessRecoveryChanged>.Invalid(issues);
        }

        DataAccessRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DataAccessRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DataAccessRecoveryChanged>.Invalid(
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

        DataAccessRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DataAccessRecoveryChanged>.Success(changed);
    }
}