namespace AtlasOps.Features.Data.DataDatasetRecovery;

using AtlasOps.Features;

public sealed class DataDatasetRecoveryService(
    IAtlasOpsCapabilityRepository<DataDatasetRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly DataDatasetRecoveryValidator validator = new();
    private readonly DataDatasetRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DataDatasetRecoveryChanged>> ExecuteAsync(
        UpdateDataDatasetRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DataDatasetRecoveryChanged>.Invalid(issues);
        }

        DataDatasetRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DataDatasetRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DataDatasetRecoveryChanged>.Invalid(
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

        DataDatasetRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DataDatasetRecoveryChanged>.Success(changed);
    }
}