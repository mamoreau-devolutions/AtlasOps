namespace AtlasOps.Features.Data.DataPipelineProvisioning;

using AtlasOps.Features;

public sealed class DataPipelineProvisioningService(
    IAtlasOpsCapabilityRepository<DataPipelineProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly DataPipelineProvisioningValidator validator = new();
    private readonly DataPipelineProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DataPipelineProvisioningChanged>> ExecuteAsync(
        UpdateDataPipelineProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DataPipelineProvisioningChanged>.Invalid(issues);
        }

        DataPipelineProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DataPipelineProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DataPipelineProvisioningChanged>.Invalid(
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

        DataPipelineProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DataPipelineProvisioningChanged>.Success(changed);
    }
}