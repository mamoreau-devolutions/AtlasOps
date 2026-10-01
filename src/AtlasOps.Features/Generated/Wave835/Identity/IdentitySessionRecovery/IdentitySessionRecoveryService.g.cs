namespace AtlasOps.Features.Identity.IdentitySessionRecovery;

using AtlasOps.Features;

public sealed class IdentitySessionRecoveryService(
    IAtlasOpsCapabilityRepository<IdentitySessionRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly IdentitySessionRecoveryValidator validator = new();
    private readonly IdentitySessionRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<IdentitySessionRecoveryChanged>> ExecuteAsync(
        UpdateIdentitySessionRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<IdentitySessionRecoveryChanged>.Invalid(issues);
        }

        IdentitySessionRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new IdentitySessionRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<IdentitySessionRecoveryChanged>.Invalid(
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

        IdentitySessionRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<IdentitySessionRecoveryChanged>.Success(changed);
    }
}