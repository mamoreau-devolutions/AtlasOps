namespace AtlasOps.Features.Security.SecurityFindingRecovery;

using AtlasOps.Features;

public sealed class SecurityFindingRecoveryService(
    IAtlasOpsCapabilityRepository<SecurityFindingRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly SecurityFindingRecoveryValidator validator = new();
    private readonly SecurityFindingRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SecurityFindingRecoveryChanged>> ExecuteAsync(
        UpdateSecurityFindingRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SecurityFindingRecoveryChanged>.Invalid(issues);
        }

        SecurityFindingRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SecurityFindingRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SecurityFindingRecoveryChanged>.Invalid(
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

        SecurityFindingRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SecurityFindingRecoveryChanged>.Success(changed);
    }
}