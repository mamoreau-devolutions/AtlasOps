namespace AtlasOps.Features.Identity.IdentityLifecycleRecovery;

using AtlasOps.Features;

public sealed class IdentityLifecycleRecoveryService(
    IAtlasOpsCapabilityRepository<IdentityLifecycleRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly IdentityLifecycleRecoveryValidator validator = new();
    private readonly IdentityLifecycleRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<IdentityLifecycleRecoveryChanged>> ExecuteAsync(
        UpdateIdentityLifecycleRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<IdentityLifecycleRecoveryChanged>.Invalid(issues);
        }

        IdentityLifecycleRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new IdentityLifecycleRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<IdentityLifecycleRecoveryChanged>.Invalid(
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

        IdentityLifecycleRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<IdentityLifecycleRecoveryChanged>.Success(changed);
    }
}