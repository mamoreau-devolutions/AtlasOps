namespace AtlasOps.Features.FinOps.SavingsPlanProvisioning;

using AtlasOps.Features;

public sealed class SavingsPlanProvisioningService(
    IAtlasOpsCapabilityRepository<SavingsPlanProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly SavingsPlanProvisioningValidator validator = new();
    private readonly SavingsPlanProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SavingsPlanProvisioningChanged>> ExecuteAsync(
        UpdateSavingsPlanProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SavingsPlanProvisioningChanged>.Invalid(issues);
        }

        SavingsPlanProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SavingsPlanProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SavingsPlanProvisioningChanged>.Invalid(
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

        SavingsPlanProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SavingsPlanProvisioningChanged>.Success(changed);
    }
}