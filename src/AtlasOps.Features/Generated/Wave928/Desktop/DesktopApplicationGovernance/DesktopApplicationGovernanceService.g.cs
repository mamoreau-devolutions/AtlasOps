namespace AtlasOps.Features.Desktop.DesktopApplicationGovernance;

using AtlasOps.Features;

public sealed class DesktopApplicationGovernanceService(
    IAtlasOpsCapabilityRepository<DesktopApplicationGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly DesktopApplicationGovernanceValidator validator = new();
    private readonly DesktopApplicationGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<DesktopApplicationGovernanceChanged>> ExecuteAsync(
        UpdateDesktopApplicationGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DesktopApplicationGovernanceChanged>.Invalid(issues);
        }

        DesktopApplicationGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DesktopApplicationGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DesktopApplicationGovernanceChanged>.Invalid(
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

        DesktopApplicationGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DesktopApplicationGovernanceChanged>.Success(changed);
    }
}