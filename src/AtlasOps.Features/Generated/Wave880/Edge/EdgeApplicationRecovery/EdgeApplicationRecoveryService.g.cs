namespace AtlasOps.Features.Edge.EdgeApplicationRecovery;

using AtlasOps.Features;

public sealed class EdgeApplicationRecoveryService(
    IAtlasOpsCapabilityRepository<EdgeApplicationRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly EdgeApplicationRecoveryValidator validator = new();
    private readonly EdgeApplicationRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<EdgeApplicationRecoveryChanged>> ExecuteAsync(
        UpdateEdgeApplicationRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<EdgeApplicationRecoveryChanged>.Invalid(issues);
        }

        EdgeApplicationRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new EdgeApplicationRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<EdgeApplicationRecoveryChanged>.Invalid(
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

        EdgeApplicationRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<EdgeApplicationRecoveryChanged>.Success(changed);
    }
}