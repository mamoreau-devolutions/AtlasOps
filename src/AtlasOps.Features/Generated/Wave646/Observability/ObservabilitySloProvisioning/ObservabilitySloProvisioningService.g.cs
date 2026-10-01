namespace AtlasOps.Features.Observability.ObservabilitySloProvisioning;

using AtlasOps.Features;

public sealed class ObservabilitySloProvisioningService(
    IAtlasOpsCapabilityRepository<ObservabilitySloProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly ObservabilitySloProvisioningValidator validator = new();
    private readonly ObservabilitySloProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ObservabilitySloProvisioningChanged>> ExecuteAsync(
        UpdateObservabilitySloProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ObservabilitySloProvisioningChanged>.Invalid(issues);
        }

        ObservabilitySloProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ObservabilitySloProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ObservabilitySloProvisioningChanged>.Invalid(
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

        ObservabilitySloProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ObservabilitySloProvisioningChanged>.Success(changed);
    }
}