namespace AtlasOps.Features.BusinessContinuity.RecoveryDependencyProvisioning;

using AtlasOps.Features;

public sealed class RecoveryDependencyProvisioningService(
    IAtlasOpsCapabilityRepository<RecoveryDependencyProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly RecoveryDependencyProvisioningValidator validator = new();
    private readonly RecoveryDependencyProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<RecoveryDependencyProvisioningChanged>> ExecuteAsync(
        UpdateRecoveryDependencyProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RecoveryDependencyProvisioningChanged>.Invalid(issues);
        }

        RecoveryDependencyProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RecoveryDependencyProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RecoveryDependencyProvisioningChanged>.Invalid(
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

        RecoveryDependencyProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RecoveryDependencyProvisioningChanged>.Success(changed);
    }
}