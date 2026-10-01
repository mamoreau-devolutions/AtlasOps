namespace AtlasOps.Features.Desktop.DesktopImageGovernance;

using AtlasOps.Features;

public sealed class DesktopImageGovernanceService(
    IAtlasOpsCapabilityRepository<DesktopImageGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly DesktopImageGovernanceValidator validator = new();
    private readonly DesktopImageGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<DesktopImageGovernanceChanged>> ExecuteAsync(
        UpdateDesktopImageGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DesktopImageGovernanceChanged>.Invalid(issues);
        }

        DesktopImageGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DesktopImageGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DesktopImageGovernanceChanged>.Invalid(
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

        DesktopImageGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DesktopImageGovernanceChanged>.Success(changed);
    }
}