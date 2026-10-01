namespace AtlasOps.Features.Desktop.DesktopPolicyGovernance;

using AtlasOps.Features;

public sealed class DesktopPolicyGovernanceService(
    IAtlasOpsCapabilityRepository<DesktopPolicyGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly DesktopPolicyGovernanceValidator validator = new();
    private readonly DesktopPolicyGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<DesktopPolicyGovernanceChanged>> ExecuteAsync(
        UpdateDesktopPolicyGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DesktopPolicyGovernanceChanged>.Invalid(issues);
        }

        DesktopPolicyGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DesktopPolicyGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DesktopPolicyGovernanceChanged>.Invalid(
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

        DesktopPolicyGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DesktopPolicyGovernanceChanged>.Success(changed);
    }
}