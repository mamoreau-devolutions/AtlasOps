namespace AtlasOps.Features.Data.DataContractRecovery;

using AtlasOps.Features;

public sealed class DataContractRecoveryService(
    IAtlasOpsCapabilityRepository<DataContractRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly DataContractRecoveryValidator validator = new();
    private readonly DataContractRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DataContractRecoveryChanged>> ExecuteAsync(
        UpdateDataContractRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DataContractRecoveryChanged>.Invalid(issues);
        }

        DataContractRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DataContractRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DataContractRecoveryChanged>.Invalid(
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

        DataContractRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DataContractRecoveryChanged>.Success(changed);
    }
}