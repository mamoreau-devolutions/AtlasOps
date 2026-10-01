namespace AtlasOps.Features.Delivery.ReleaseApprovalProvisioning;

using AtlasOps.Features;

public sealed class ReleaseApprovalProvisioningService(
    IAtlasOpsCapabilityRepository<ReleaseApprovalProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly ReleaseApprovalProvisioningValidator validator = new();
    private readonly ReleaseApprovalProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ReleaseApprovalProvisioningChanged>> ExecuteAsync(
        UpdateReleaseApprovalProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ReleaseApprovalProvisioningChanged>.Invalid(issues);
        }

        ReleaseApprovalProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ReleaseApprovalProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ReleaseApprovalProvisioningChanged>.Invalid(
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

        ReleaseApprovalProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ReleaseApprovalProvisioningChanged>.Success(changed);
    }
}