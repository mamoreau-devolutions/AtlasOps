namespace AtlasOps.Features.Desktop.DesktopLicenseRecovery;

using AtlasOps.Features;

public sealed class DesktopLicenseRecoveryService(
    IAtlasOpsCapabilityRepository<DesktopLicenseRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly DesktopLicenseRecoveryValidator validator = new();
    private readonly DesktopLicenseRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DesktopLicenseRecoveryChanged>> ExecuteAsync(
        UpdateDesktopLicenseRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DesktopLicenseRecoveryChanged>.Invalid(issues);
        }

        DesktopLicenseRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DesktopLicenseRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DesktopLicenseRecoveryChanged>.Invalid(
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

        DesktopLicenseRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DesktopLicenseRecoveryChanged>.Success(changed);
    }
}