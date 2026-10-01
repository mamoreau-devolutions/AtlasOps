namespace AtlasOps.Features.Edge.EdgePolicyRecovery;

using AtlasOps.Features;

public sealed class EdgePolicyRecoveryService(
    IAtlasOpsCapabilityRepository<EdgePolicyRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly EdgePolicyRecoveryValidator validator = new();
    private readonly EdgePolicyRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<EdgePolicyRecoveryChanged>> ExecuteAsync(
        UpdateEdgePolicyRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<EdgePolicyRecoveryChanged>.Invalid(issues);
        }

        EdgePolicyRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new EdgePolicyRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<EdgePolicyRecoveryChanged>.Invalid(
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

        EdgePolicyRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<EdgePolicyRecoveryChanged>.Success(changed);
    }
}