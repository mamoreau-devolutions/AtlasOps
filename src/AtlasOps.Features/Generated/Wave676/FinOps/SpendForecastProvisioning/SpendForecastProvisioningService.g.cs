namespace AtlasOps.Features.FinOps.SpendForecastProvisioning;

using AtlasOps.Features;

public sealed class SpendForecastProvisioningService(
    IAtlasOpsCapabilityRepository<SpendForecastProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly SpendForecastProvisioningValidator validator = new();
    private readonly SpendForecastProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SpendForecastProvisioningChanged>> ExecuteAsync(
        UpdateSpendForecastProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SpendForecastProvisioningChanged>.Invalid(issues);
        }

        SpendForecastProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SpendForecastProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SpendForecastProvisioningChanged>.Invalid(
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

        SpendForecastProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SpendForecastProvisioningChanged>.Success(changed);
    }
}