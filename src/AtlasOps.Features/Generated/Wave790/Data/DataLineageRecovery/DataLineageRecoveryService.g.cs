namespace AtlasOps.Features.Data.DataLineageRecovery;

using AtlasOps.Features;

public sealed class DataLineageRecoveryService(
    IAtlasOpsCapabilityRepository<DataLineageRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly DataLineageRecoveryValidator validator = new();
    private readonly DataLineageRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DataLineageRecoveryChanged>> ExecuteAsync(
        UpdateDataLineageRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DataLineageRecoveryChanged>.Invalid(issues);
        }

        DataLineageRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DataLineageRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DataLineageRecoveryChanged>.Invalid(
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

        DataLineageRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DataLineageRecoveryChanged>.Success(changed);
    }
}