namespace AtlasOps.Features.Security.SecuritySessionRecovery;

using AtlasOps.Features;

public sealed class SecuritySessionRecoveryService(
    IAtlasOpsCapabilityRepository<SecuritySessionRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly SecuritySessionRecoveryValidator validator = new();
    private readonly SecuritySessionRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SecuritySessionRecoveryChanged>> ExecuteAsync(
        UpdateSecuritySessionRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SecuritySessionRecoveryChanged>.Invalid(issues);
        }

        SecuritySessionRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SecuritySessionRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SecuritySessionRecoveryChanged>.Invalid(
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

        SecuritySessionRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SecuritySessionRecoveryChanged>.Success(changed);
    }
}