namespace AtlasOps.Features.Data.DataPipelineRecovery;

using AtlasOps.Features;

public sealed class DataPipelineRecoveryService(
    IAtlasOpsCapabilityRepository<DataPipelineRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly DataPipelineRecoveryValidator validator = new();
    private readonly DataPipelineRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DataPipelineRecoveryChanged>> ExecuteAsync(
        UpdateDataPipelineRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DataPipelineRecoveryChanged>.Invalid(issues);
        }

        DataPipelineRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DataPipelineRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DataPipelineRecoveryChanged>.Invalid(
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

        DataPipelineRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DataPipelineRecoveryChanged>.Success(changed);
    }
}