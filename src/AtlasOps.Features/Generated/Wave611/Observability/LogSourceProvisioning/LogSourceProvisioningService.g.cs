namespace AtlasOps.Features.Observability.LogSourceProvisioning;

using AtlasOps.Features;

public sealed class LogSourceProvisioningService(
    IAtlasOpsCapabilityRepository<LogSourceProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly LogSourceProvisioningValidator validator = new();
    private readonly LogSourceProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<LogSourceProvisioningChanged>> ExecuteAsync(
        UpdateLogSourceProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<LogSourceProvisioningChanged>.Invalid(issues);
        }

        LogSourceProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new LogSourceProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<LogSourceProvisioningChanged>.Invalid(
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

        LogSourceProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<LogSourceProvisioningChanged>.Success(changed);
    }
}