namespace AtlasOps.Features.Desktop.DesktopSessionGovernance;

using AtlasOps.Features;

public sealed class DesktopSessionGovernanceService(
    IAtlasOpsCapabilityRepository<DesktopSessionGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly DesktopSessionGovernanceValidator validator = new();
    private readonly DesktopSessionGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<DesktopSessionGovernanceChanged>> ExecuteAsync(
        UpdateDesktopSessionGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DesktopSessionGovernanceChanged>.Invalid(issues);
        }

        DesktopSessionGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DesktopSessionGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DesktopSessionGovernanceChanged>.Invalid(
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

        DesktopSessionGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DesktopSessionGovernanceChanged>.Success(changed);
    }
}