namespace AtlasOps.Features.Identity.IdentityApplicationRecovery;

using AtlasOps.Features;

public sealed class IdentityApplicationRecoveryService(
    IAtlasOpsCapabilityRepository<IdentityApplicationRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly IdentityApplicationRecoveryValidator validator = new();
    private readonly IdentityApplicationRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<IdentityApplicationRecoveryChanged>> ExecuteAsync(
        UpdateIdentityApplicationRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<IdentityApplicationRecoveryChanged>.Invalid(issues);
        }

        IdentityApplicationRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new IdentityApplicationRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<IdentityApplicationRecoveryChanged>.Invalid(
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

        IdentityApplicationRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<IdentityApplicationRecoveryChanged>.Success(changed);
    }
}