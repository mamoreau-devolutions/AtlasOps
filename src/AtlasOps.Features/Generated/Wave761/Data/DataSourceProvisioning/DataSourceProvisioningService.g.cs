namespace AtlasOps.Features.Data.DataSourceProvisioning;

using AtlasOps.Features;

public sealed class DataSourceProvisioningService(
    IAtlasOpsCapabilityRepository<DataSourceProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly DataSourceProvisioningValidator validator = new();
    private readonly DataSourceProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DataSourceProvisioningChanged>> ExecuteAsync(
        UpdateDataSourceProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DataSourceProvisioningChanged>.Invalid(issues);
        }

        DataSourceProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DataSourceProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DataSourceProvisioningChanged>.Invalid(
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

        DataSourceProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DataSourceProvisioningChanged>.Success(changed);
    }
}