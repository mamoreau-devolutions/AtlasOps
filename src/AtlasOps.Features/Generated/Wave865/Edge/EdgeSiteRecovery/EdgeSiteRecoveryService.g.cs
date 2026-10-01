namespace AtlasOps.Features.Edge.EdgeSiteRecovery;

using AtlasOps.Features;

public sealed class EdgeSiteRecoveryService(
    IAtlasOpsCapabilityRepository<EdgeSiteRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly EdgeSiteRecoveryValidator validator = new();
    private readonly EdgeSiteRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<EdgeSiteRecoveryChanged>> ExecuteAsync(
        UpdateEdgeSiteRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<EdgeSiteRecoveryChanged>.Invalid(issues);
        }

        EdgeSiteRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new EdgeSiteRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<EdgeSiteRecoveryChanged>.Invalid(
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

        EdgeSiteRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<EdgeSiteRecoveryChanged>.Success(changed);
    }
}