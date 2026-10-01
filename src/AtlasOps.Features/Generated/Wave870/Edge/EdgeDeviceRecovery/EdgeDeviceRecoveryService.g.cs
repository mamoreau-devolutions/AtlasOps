namespace AtlasOps.Features.Edge.EdgeDeviceRecovery;

using AtlasOps.Features;

public sealed class EdgeDeviceRecoveryService(
    IAtlasOpsCapabilityRepository<EdgeDeviceRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly EdgeDeviceRecoveryValidator validator = new();
    private readonly EdgeDeviceRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<EdgeDeviceRecoveryChanged>> ExecuteAsync(
        UpdateEdgeDeviceRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<EdgeDeviceRecoveryChanged>.Invalid(issues);
        }

        EdgeDeviceRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new EdgeDeviceRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<EdgeDeviceRecoveryChanged>.Invalid(
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

        EdgeDeviceRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<EdgeDeviceRecoveryChanged>.Success(changed);
    }
}