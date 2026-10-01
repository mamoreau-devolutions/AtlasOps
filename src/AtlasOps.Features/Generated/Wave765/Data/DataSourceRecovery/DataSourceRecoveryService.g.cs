namespace AtlasOps.Features.Data.DataSourceRecovery;

using AtlasOps.Features;

public sealed class DataSourceRecoveryService(
    IAtlasOpsCapabilityRepository<DataSourceRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly DataSourceRecoveryValidator validator = new();
    private readonly DataSourceRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DataSourceRecoveryChanged>> ExecuteAsync(
        UpdateDataSourceRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DataSourceRecoveryChanged>.Invalid(issues);
        }

        DataSourceRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DataSourceRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DataSourceRecoveryChanged>.Invalid(
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

        DataSourceRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DataSourceRecoveryChanged>.Success(changed);
    }
}