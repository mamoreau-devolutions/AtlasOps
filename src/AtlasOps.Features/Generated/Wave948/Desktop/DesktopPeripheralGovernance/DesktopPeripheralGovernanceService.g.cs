namespace AtlasOps.Features.Desktop.DesktopPeripheralGovernance;

using AtlasOps.Features;

public sealed class DesktopPeripheralGovernanceService(
    IAtlasOpsCapabilityRepository<DesktopPeripheralGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly DesktopPeripheralGovernanceValidator validator = new();
    private readonly DesktopPeripheralGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<DesktopPeripheralGovernanceChanged>> ExecuteAsync(
        UpdateDesktopPeripheralGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DesktopPeripheralGovernanceChanged>.Invalid(issues);
        }

        DesktopPeripheralGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DesktopPeripheralGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DesktopPeripheralGovernanceChanged>.Invalid(
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

        DesktopPeripheralGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DesktopPeripheralGovernanceChanged>.Success(changed);
    }
}