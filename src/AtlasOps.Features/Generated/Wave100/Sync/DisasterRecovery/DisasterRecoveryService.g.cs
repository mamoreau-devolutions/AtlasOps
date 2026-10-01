namespace AtlasOps.Features.Sync.DisasterRecovery;

using AtlasOps.Features;

public sealed class DisasterRecoveryService(
    IAtlasOpsCapabilityRepository<DisasterRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly DisasterRecoveryValidator validator = new();
    private readonly DisasterRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DisasterRecoveryChanged>> ExecuteAsync(
        UpdateDisasterRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DisasterRecoveryChanged>.Invalid(issues);
        }

        DisasterRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DisasterRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DisasterRecoveryChanged>.Invalid(
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

        DisasterRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DisasterRecoveryChanged>.Success(changed);
    }
}