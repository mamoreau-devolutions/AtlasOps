namespace AtlasOps.Features.Desktop.DesktopUpdateGovernance;

using AtlasOps.Features;

public sealed class DesktopUpdateGovernanceService(
    IAtlasOpsCapabilityRepository<DesktopUpdateGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly DesktopUpdateGovernanceValidator validator = new();
    private readonly DesktopUpdateGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<DesktopUpdateGovernanceChanged>> ExecuteAsync(
        UpdateDesktopUpdateGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DesktopUpdateGovernanceChanged>.Invalid(issues);
        }

        DesktopUpdateGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DesktopUpdateGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DesktopUpdateGovernanceChanged>.Invalid(
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

        DesktopUpdateGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DesktopUpdateGovernanceChanged>.Success(changed);
    }
}