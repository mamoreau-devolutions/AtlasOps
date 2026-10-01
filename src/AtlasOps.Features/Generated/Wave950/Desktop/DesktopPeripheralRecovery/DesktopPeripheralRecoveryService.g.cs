namespace AtlasOps.Features.Desktop.DesktopPeripheralRecovery;

using AtlasOps.Features;

public sealed class DesktopPeripheralRecoveryService(
    IAtlasOpsCapabilityRepository<DesktopPeripheralRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly DesktopPeripheralRecoveryValidator validator = new();
    private readonly DesktopPeripheralRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DesktopPeripheralRecoveryChanged>> ExecuteAsync(
        UpdateDesktopPeripheralRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DesktopPeripheralRecoveryChanged>.Invalid(issues);
        }

        DesktopPeripheralRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DesktopPeripheralRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DesktopPeripheralRecoveryChanged>.Invalid(
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

        DesktopPeripheralRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DesktopPeripheralRecoveryChanged>.Success(changed);
    }
}