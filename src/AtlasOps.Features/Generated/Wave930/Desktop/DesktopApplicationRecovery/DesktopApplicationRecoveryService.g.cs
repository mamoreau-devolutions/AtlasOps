namespace AtlasOps.Features.Desktop.DesktopApplicationRecovery;

using AtlasOps.Features;

public sealed class DesktopApplicationRecoveryService(
    IAtlasOpsCapabilityRepository<DesktopApplicationRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly DesktopApplicationRecoveryValidator validator = new();
    private readonly DesktopApplicationRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DesktopApplicationRecoveryChanged>> ExecuteAsync(
        UpdateDesktopApplicationRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DesktopApplicationRecoveryChanged>.Invalid(issues);
        }

        DesktopApplicationRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DesktopApplicationRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DesktopApplicationRecoveryChanged>.Invalid(
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

        DesktopApplicationRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DesktopApplicationRecoveryChanged>.Success(changed);
    }
}