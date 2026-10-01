namespace AtlasOps.Features.Security.SecurityExceptionRecovery;

using AtlasOps.Features;

public sealed class SecurityExceptionRecoveryService(
    IAtlasOpsCapabilityRepository<SecurityExceptionRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly SecurityExceptionRecoveryValidator validator = new();
    private readonly SecurityExceptionRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SecurityExceptionRecoveryChanged>> ExecuteAsync(
        UpdateSecurityExceptionRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SecurityExceptionRecoveryChanged>.Invalid(issues);
        }

        SecurityExceptionRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SecurityExceptionRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SecurityExceptionRecoveryChanged>.Invalid(
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

        SecurityExceptionRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SecurityExceptionRecoveryChanged>.Success(changed);
    }
}