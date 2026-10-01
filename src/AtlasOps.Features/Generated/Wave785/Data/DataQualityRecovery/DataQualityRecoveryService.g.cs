namespace AtlasOps.Features.Data.DataQualityRecovery;

using AtlasOps.Features;

public sealed class DataQualityRecoveryService(
    IAtlasOpsCapabilityRepository<DataQualityRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly DataQualityRecoveryValidator validator = new();
    private readonly DataQualityRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DataQualityRecoveryChanged>> ExecuteAsync(
        UpdateDataQualityRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DataQualityRecoveryChanged>.Invalid(issues);
        }

        DataQualityRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DataQualityRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DataQualityRecoveryChanged>.Invalid(
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

        DataQualityRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DataQualityRecoveryChanged>.Success(changed);
    }
}