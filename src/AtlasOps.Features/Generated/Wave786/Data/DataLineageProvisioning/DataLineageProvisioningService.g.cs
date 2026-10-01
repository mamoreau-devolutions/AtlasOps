namespace AtlasOps.Features.Data.DataLineageProvisioning;

using AtlasOps.Features;

public sealed class DataLineageProvisioningService(
    IAtlasOpsCapabilityRepository<DataLineageProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly DataLineageProvisioningValidator validator = new();
    private readonly DataLineageProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DataLineageProvisioningChanged>> ExecuteAsync(
        UpdateDataLineageProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DataLineageProvisioningChanged>.Invalid(issues);
        }

        DataLineageProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DataLineageProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DataLineageProvisioningChanged>.Invalid(
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

        DataLineageProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DataLineageProvisioningChanged>.Success(changed);
    }
}