namespace AtlasOps.Features.Desktop.DesktopUpdateRecovery;

using AtlasOps.Features;

public sealed class DesktopUpdateRecoveryService(
    IAtlasOpsCapabilityRepository<DesktopUpdateRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly DesktopUpdateRecoveryValidator validator = new();
    private readonly DesktopUpdateRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DesktopUpdateRecoveryChanged>> ExecuteAsync(
        UpdateDesktopUpdateRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DesktopUpdateRecoveryChanged>.Invalid(issues);
        }

        DesktopUpdateRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DesktopUpdateRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DesktopUpdateRecoveryChanged>.Invalid(
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

        DesktopUpdateRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DesktopUpdateRecoveryChanged>.Success(changed);
    }
}