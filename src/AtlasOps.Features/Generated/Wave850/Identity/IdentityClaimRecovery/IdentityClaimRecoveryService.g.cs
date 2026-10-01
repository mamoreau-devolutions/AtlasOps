namespace AtlasOps.Features.Identity.IdentityClaimRecovery;

using AtlasOps.Features;

public sealed class IdentityClaimRecoveryService(
    IAtlasOpsCapabilityRepository<IdentityClaimRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly IdentityClaimRecoveryValidator validator = new();
    private readonly IdentityClaimRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<IdentityClaimRecoveryChanged>> ExecuteAsync(
        UpdateIdentityClaimRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<IdentityClaimRecoveryChanged>.Invalid(issues);
        }

        IdentityClaimRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new IdentityClaimRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<IdentityClaimRecoveryChanged>.Invalid(
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

        IdentityClaimRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<IdentityClaimRecoveryChanged>.Success(changed);
    }
}