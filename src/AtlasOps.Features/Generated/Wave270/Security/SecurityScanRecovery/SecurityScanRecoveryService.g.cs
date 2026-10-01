namespace AtlasOps.Features.Security.SecurityScanRecovery;

using AtlasOps.Features;

public sealed class SecurityScanRecoveryService(
    IAtlasOpsCapabilityRepository<SecurityScanRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly SecurityScanRecoveryValidator validator = new();
    private readonly SecurityScanRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SecurityScanRecoveryChanged>> ExecuteAsync(
        UpdateSecurityScanRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SecurityScanRecoveryChanged>.Invalid(issues);
        }

        SecurityScanRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SecurityScanRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SecurityScanRecoveryChanged>.Invalid(
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

        SecurityScanRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SecurityScanRecoveryChanged>.Success(changed);
    }
}