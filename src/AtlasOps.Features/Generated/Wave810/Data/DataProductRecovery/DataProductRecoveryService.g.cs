namespace AtlasOps.Features.Data.DataProductRecovery;

using AtlasOps.Features;

public sealed class DataProductRecoveryService(
    IAtlasOpsCapabilityRepository<DataProductRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly DataProductRecoveryValidator validator = new();
    private readonly DataProductRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DataProductRecoveryChanged>> ExecuteAsync(
        UpdateDataProductRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DataProductRecoveryChanged>.Invalid(issues);
        }

        DataProductRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DataProductRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DataProductRecoveryChanged>.Invalid(
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

        DataProductRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DataProductRecoveryChanged>.Success(changed);
    }
}