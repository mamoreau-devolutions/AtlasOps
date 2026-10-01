namespace AtlasOps.Features.Security.SecurityKeyRecovery;

using AtlasOps.Features;

public sealed class SecurityKeyRecoveryService(
    IAtlasOpsCapabilityRepository<SecurityKeyRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly SecurityKeyRecoveryValidator validator = new();
    private readonly SecurityKeyRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SecurityKeyRecoveryChanged>> ExecuteAsync(
        UpdateSecurityKeyRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SecurityKeyRecoveryChanged>.Invalid(issues);
        }

        SecurityKeyRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SecurityKeyRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SecurityKeyRecoveryChanged>.Invalid(
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

        SecurityKeyRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SecurityKeyRecoveryChanged>.Success(changed);
    }
}