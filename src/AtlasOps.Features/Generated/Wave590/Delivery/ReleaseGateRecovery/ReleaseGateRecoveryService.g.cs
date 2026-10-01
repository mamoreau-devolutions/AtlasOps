namespace AtlasOps.Features.Delivery.ReleaseGateRecovery;

using AtlasOps.Features;

public sealed class ReleaseGateRecoveryService(
    IAtlasOpsCapabilityRepository<ReleaseGateRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly ReleaseGateRecoveryValidator validator = new();
    private readonly ReleaseGateRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ReleaseGateRecoveryChanged>> ExecuteAsync(
        UpdateReleaseGateRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ReleaseGateRecoveryChanged>.Invalid(issues);
        }

        ReleaseGateRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ReleaseGateRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ReleaseGateRecoveryChanged>.Invalid(
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

        ReleaseGateRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ReleaseGateRecoveryChanged>.Success(changed);
    }
}