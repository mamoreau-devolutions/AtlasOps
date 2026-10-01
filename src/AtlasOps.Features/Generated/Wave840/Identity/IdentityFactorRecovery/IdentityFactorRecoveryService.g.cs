namespace AtlasOps.Features.Identity.IdentityFactorRecovery;

using AtlasOps.Features;

public sealed class IdentityFactorRecoveryService(
    IAtlasOpsCapabilityRepository<IdentityFactorRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly IdentityFactorRecoveryValidator validator = new();
    private readonly IdentityFactorRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<IdentityFactorRecoveryChanged>> ExecuteAsync(
        UpdateIdentityFactorRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<IdentityFactorRecoveryChanged>.Invalid(issues);
        }

        IdentityFactorRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new IdentityFactorRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<IdentityFactorRecoveryChanged>.Invalid(
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

        IdentityFactorRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<IdentityFactorRecoveryChanged>.Success(changed);
    }
}