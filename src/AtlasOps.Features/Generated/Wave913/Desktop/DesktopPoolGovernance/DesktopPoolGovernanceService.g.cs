namespace AtlasOps.Features.Desktop.DesktopPoolGovernance;

using AtlasOps.Features;

public sealed class DesktopPoolGovernanceService(
    IAtlasOpsCapabilityRepository<DesktopPoolGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly DesktopPoolGovernanceValidator validator = new();
    private readonly DesktopPoolGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<DesktopPoolGovernanceChanged>> ExecuteAsync(
        UpdateDesktopPoolGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DesktopPoolGovernanceChanged>.Invalid(issues);
        }

        DesktopPoolGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DesktopPoolGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DesktopPoolGovernanceChanged>.Invalid(
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

        DesktopPoolGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DesktopPoolGovernanceChanged>.Success(changed);
    }
}