namespace AtlasOps.Features.Desktop.DesktopLicenseGovernance;

using AtlasOps.Features;

public sealed class DesktopLicenseGovernanceService(
    IAtlasOpsCapabilityRepository<DesktopLicenseGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly DesktopLicenseGovernanceValidator validator = new();
    private readonly DesktopLicenseGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<DesktopLicenseGovernanceChanged>> ExecuteAsync(
        UpdateDesktopLicenseGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DesktopLicenseGovernanceChanged>.Invalid(issues);
        }

        DesktopLicenseGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DesktopLicenseGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DesktopLicenseGovernanceChanged>.Invalid(
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

        DesktopLicenseGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DesktopLicenseGovernanceChanged>.Success(changed);
    }
}