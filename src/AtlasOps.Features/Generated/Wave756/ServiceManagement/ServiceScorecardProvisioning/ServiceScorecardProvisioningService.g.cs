namespace AtlasOps.Features.ServiceManagement.ServiceScorecardProvisioning;

using AtlasOps.Features;

public sealed class ServiceScorecardProvisioningService(
    IAtlasOpsCapabilityRepository<ServiceScorecardProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly ServiceScorecardProvisioningValidator validator = new();
    private readonly ServiceScorecardProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ServiceScorecardProvisioningChanged>> ExecuteAsync(
        UpdateServiceScorecardProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ServiceScorecardProvisioningChanged>.Invalid(issues);
        }

        ServiceScorecardProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ServiceScorecardProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ServiceScorecardProvisioningChanged>.Invalid(
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

        ServiceScorecardProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ServiceScorecardProvisioningChanged>.Success(changed);
    }
}