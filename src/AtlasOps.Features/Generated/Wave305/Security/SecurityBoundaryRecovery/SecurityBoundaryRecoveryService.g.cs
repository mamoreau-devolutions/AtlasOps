namespace AtlasOps.Features.Security.SecurityBoundaryRecovery;

using AtlasOps.Features;

public sealed class SecurityBoundaryRecoveryService(
    IAtlasOpsCapabilityRepository<SecurityBoundaryRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly SecurityBoundaryRecoveryValidator validator = new();
    private readonly SecurityBoundaryRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SecurityBoundaryRecoveryChanged>> ExecuteAsync(
        UpdateSecurityBoundaryRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SecurityBoundaryRecoveryChanged>.Invalid(issues);
        }

        SecurityBoundaryRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SecurityBoundaryRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SecurityBoundaryRecoveryChanged>.Invalid(
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

        SecurityBoundaryRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SecurityBoundaryRecoveryChanged>.Success(changed);
    }
}