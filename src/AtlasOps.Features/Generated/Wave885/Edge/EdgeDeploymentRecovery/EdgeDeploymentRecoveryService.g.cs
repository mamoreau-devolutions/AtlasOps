namespace AtlasOps.Features.Edge.EdgeDeploymentRecovery;

using AtlasOps.Features;

public sealed class EdgeDeploymentRecoveryService(
    IAtlasOpsCapabilityRepository<EdgeDeploymentRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly EdgeDeploymentRecoveryValidator validator = new();
    private readonly EdgeDeploymentRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<EdgeDeploymentRecoveryChanged>> ExecuteAsync(
        UpdateEdgeDeploymentRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<EdgeDeploymentRecoveryChanged>.Invalid(issues);
        }

        EdgeDeploymentRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new EdgeDeploymentRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<EdgeDeploymentRecoveryChanged>.Invalid(
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

        EdgeDeploymentRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<EdgeDeploymentRecoveryChanged>.Success(changed);
    }
}