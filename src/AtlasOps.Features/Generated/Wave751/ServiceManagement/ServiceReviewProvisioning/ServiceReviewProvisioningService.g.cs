namespace AtlasOps.Features.ServiceManagement.ServiceReviewProvisioning;

using AtlasOps.Features;

public sealed class ServiceReviewProvisioningService(
    IAtlasOpsCapabilityRepository<ServiceReviewProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly ServiceReviewProvisioningValidator validator = new();
    private readonly ServiceReviewProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ServiceReviewProvisioningChanged>> ExecuteAsync(
        UpdateServiceReviewProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ServiceReviewProvisioningChanged>.Invalid(issues);
        }

        ServiceReviewProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ServiceReviewProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ServiceReviewProvisioningChanged>.Invalid(
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

        ServiceReviewProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ServiceReviewProvisioningChanged>.Success(changed);
    }
}