namespace AtlasOps.Features.Desktop.DesktopPolicyRecovery;

using AtlasOps.Features;

public sealed class DesktopPolicyRecoveryService(
    IAtlasOpsCapabilityRepository<DesktopPolicyRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly DesktopPolicyRecoveryValidator validator = new();
    private readonly DesktopPolicyRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DesktopPolicyRecoveryChanged>> ExecuteAsync(
        UpdateDesktopPolicyRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DesktopPolicyRecoveryChanged>.Invalid(issues);
        }

        DesktopPolicyRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DesktopPolicyRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DesktopPolicyRecoveryChanged>.Invalid(
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

        DesktopPolicyRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DesktopPolicyRecoveryChanged>.Success(changed);
    }
}