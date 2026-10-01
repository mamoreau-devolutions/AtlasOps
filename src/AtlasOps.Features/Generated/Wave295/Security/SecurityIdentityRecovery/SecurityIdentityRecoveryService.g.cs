namespace AtlasOps.Features.Security.SecurityIdentityRecovery;

using AtlasOps.Features;

public sealed class SecurityIdentityRecoveryService(
    IAtlasOpsCapabilityRepository<SecurityIdentityRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly SecurityIdentityRecoveryValidator validator = new();
    private readonly SecurityIdentityRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SecurityIdentityRecoveryChanged>> ExecuteAsync(
        UpdateSecurityIdentityRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SecurityIdentityRecoveryChanged>.Invalid(issues);
        }

        SecurityIdentityRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SecurityIdentityRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SecurityIdentityRecoveryChanged>.Invalid(
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

        SecurityIdentityRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SecurityIdentityRecoveryChanged>.Success(changed);
    }
}