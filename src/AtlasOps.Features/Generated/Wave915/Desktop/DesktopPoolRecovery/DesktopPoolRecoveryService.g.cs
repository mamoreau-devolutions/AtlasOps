namespace AtlasOps.Features.Desktop.DesktopPoolRecovery;

using AtlasOps.Features;

public sealed class DesktopPoolRecoveryService(
    IAtlasOpsCapabilityRepository<DesktopPoolRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly DesktopPoolRecoveryValidator validator = new();
    private readonly DesktopPoolRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DesktopPoolRecoveryChanged>> ExecuteAsync(
        UpdateDesktopPoolRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DesktopPoolRecoveryChanged>.Invalid(issues);
        }

        DesktopPoolRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DesktopPoolRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DesktopPoolRecoveryChanged>.Invalid(
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

        DesktopPoolRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DesktopPoolRecoveryChanged>.Success(changed);
    }
}