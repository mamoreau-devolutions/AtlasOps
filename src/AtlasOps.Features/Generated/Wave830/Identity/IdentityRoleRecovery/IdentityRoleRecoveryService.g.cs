namespace AtlasOps.Features.Identity.IdentityRoleRecovery;

using AtlasOps.Features;

public sealed class IdentityRoleRecoveryService(
    IAtlasOpsCapabilityRepository<IdentityRoleRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly IdentityRoleRecoveryValidator validator = new();
    private readonly IdentityRoleRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<IdentityRoleRecoveryChanged>> ExecuteAsync(
        UpdateIdentityRoleRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<IdentityRoleRecoveryChanged>.Invalid(issues);
        }

        IdentityRoleRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new IdentityRoleRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<IdentityRoleRecoveryChanged>.Invalid(
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

        IdentityRoleRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<IdentityRoleRecoveryChanged>.Success(changed);
    }
}