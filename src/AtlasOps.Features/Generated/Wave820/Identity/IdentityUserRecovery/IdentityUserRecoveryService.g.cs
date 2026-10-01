namespace AtlasOps.Features.Identity.IdentityUserRecovery;

using AtlasOps.Features;

public sealed class IdentityUserRecoveryService(
    IAtlasOpsCapabilityRepository<IdentityUserRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly IdentityUserRecoveryValidator validator = new();
    private readonly IdentityUserRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<IdentityUserRecoveryChanged>> ExecuteAsync(
        UpdateIdentityUserRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<IdentityUserRecoveryChanged>.Invalid(issues);
        }

        IdentityUserRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new IdentityUserRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<IdentityUserRecoveryChanged>.Invalid(
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

        IdentityUserRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<IdentityUserRecoveryChanged>.Success(changed);
    }
}