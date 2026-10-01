namespace AtlasOps.Features.Mobile.MobileComplianceOptimization;

using AtlasOps.Features;

public sealed class MobileComplianceOptimizationService(
    IAtlasOpsCapabilityRepository<MobileComplianceOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly MobileComplianceOptimizationValidator validator = new();
    private readonly MobileComplianceOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MobileComplianceOptimizationChanged>> ExecuteAsync(
        UpdateMobileComplianceOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MobileComplianceOptimizationChanged>.Invalid(issues);
        }

        MobileComplianceOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MobileComplianceOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MobileComplianceOptimizationChanged>.Invalid(
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

        MobileComplianceOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MobileComplianceOptimizationChanged>.Success(changed);
    }
}