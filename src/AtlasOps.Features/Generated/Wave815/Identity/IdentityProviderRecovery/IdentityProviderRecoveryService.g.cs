namespace AtlasOps.Features.Identity.IdentityProviderRecovery;

using AtlasOps.Features;

public sealed class IdentityProviderRecoveryService(
    IAtlasOpsCapabilityRepository<IdentityProviderRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly IdentityProviderRecoveryValidator validator = new();
    private readonly IdentityProviderRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<IdentityProviderRecoveryChanged>> ExecuteAsync(
        UpdateIdentityProviderRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<IdentityProviderRecoveryChanged>.Invalid(issues);
        }

        IdentityProviderRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new IdentityProviderRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<IdentityProviderRecoveryChanged>.Invalid(
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

        IdentityProviderRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<IdentityProviderRecoveryChanged>.Success(changed);
    }
}