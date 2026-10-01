namespace AtlasOps.Features.Edge.EdgeApplicationProvisioning;

using AtlasOps.Features;

public sealed class EdgeApplicationProvisioningService(
    IAtlasOpsCapabilityRepository<EdgeApplicationProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly EdgeApplicationProvisioningValidator validator = new();
    private readonly EdgeApplicationProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<EdgeApplicationProvisioningChanged>> ExecuteAsync(
        UpdateEdgeApplicationProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<EdgeApplicationProvisioningChanged>.Invalid(issues);
        }

        EdgeApplicationProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new EdgeApplicationProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<EdgeApplicationProvisioningChanged>.Invalid(
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

        EdgeApplicationProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<EdgeApplicationProvisioningChanged>.Success(changed);
    }
}