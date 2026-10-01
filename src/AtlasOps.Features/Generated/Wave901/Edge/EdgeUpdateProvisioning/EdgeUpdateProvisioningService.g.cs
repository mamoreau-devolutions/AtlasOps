namespace AtlasOps.Features.Edge.EdgeUpdateProvisioning;

using AtlasOps.Features;

public sealed class EdgeUpdateProvisioningService(
    IAtlasOpsCapabilityRepository<EdgeUpdateProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly EdgeUpdateProvisioningValidator validator = new();
    private readonly EdgeUpdateProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<EdgeUpdateProvisioningChanged>> ExecuteAsync(
        UpdateEdgeUpdateProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<EdgeUpdateProvisioningChanged>.Invalid(issues);
        }

        EdgeUpdateProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new EdgeUpdateProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<EdgeUpdateProvisioningChanged>.Invalid(
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

        EdgeUpdateProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<EdgeUpdateProvisioningChanged>.Success(changed);
    }
}