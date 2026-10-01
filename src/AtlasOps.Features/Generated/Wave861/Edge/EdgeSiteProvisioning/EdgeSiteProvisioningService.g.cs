namespace AtlasOps.Features.Edge.EdgeSiteProvisioning;

using AtlasOps.Features;

public sealed class EdgeSiteProvisioningService(
    IAtlasOpsCapabilityRepository<EdgeSiteProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly EdgeSiteProvisioningValidator validator = new();
    private readonly EdgeSiteProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<EdgeSiteProvisioningChanged>> ExecuteAsync(
        UpdateEdgeSiteProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<EdgeSiteProvisioningChanged>.Invalid(issues);
        }

        EdgeSiteProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new EdgeSiteProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<EdgeSiteProvisioningChanged>.Invalid(
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

        EdgeSiteProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<EdgeSiteProvisioningChanged>.Success(changed);
    }
}