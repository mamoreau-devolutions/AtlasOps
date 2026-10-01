namespace AtlasOps.Features.Desktop.DesktopProfileGovernance;

using AtlasOps.Features;

public sealed class DesktopProfileGovernanceService(
    IAtlasOpsCapabilityRepository<DesktopProfileGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly DesktopProfileGovernanceValidator validator = new();
    private readonly DesktopProfileGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<DesktopProfileGovernanceChanged>> ExecuteAsync(
        UpdateDesktopProfileGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DesktopProfileGovernanceChanged>.Invalid(issues);
        }

        DesktopProfileGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DesktopProfileGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DesktopProfileGovernanceChanged>.Invalid(
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

        DesktopProfileGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DesktopProfileGovernanceChanged>.Success(changed);
    }
}