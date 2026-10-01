namespace AtlasOps.Features.Desktop.DesktopLicenseOptimization;

using AtlasOps.Features;

public sealed class DesktopLicenseOptimizationService(
    IAtlasOpsCapabilityRepository<DesktopLicenseOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly DesktopLicenseOptimizationValidator validator = new();
    private readonly DesktopLicenseOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DesktopLicenseOptimizationChanged>> ExecuteAsync(
        UpdateDesktopLicenseOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DesktopLicenseOptimizationChanged>.Invalid(issues);
        }

        DesktopLicenseOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DesktopLicenseOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DesktopLicenseOptimizationChanged>.Invalid(
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

        DesktopLicenseOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DesktopLicenseOptimizationChanged>.Success(changed);
    }
}