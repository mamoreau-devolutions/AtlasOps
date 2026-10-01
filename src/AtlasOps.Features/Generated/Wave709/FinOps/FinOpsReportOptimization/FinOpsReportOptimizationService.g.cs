namespace AtlasOps.Features.FinOps.FinOpsReportOptimization;

using AtlasOps.Features;

public sealed class FinOpsReportOptimizationService(
    IAtlasOpsCapabilityRepository<FinOpsReportOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly FinOpsReportOptimizationValidator validator = new();
    private readonly FinOpsReportOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<FinOpsReportOptimizationChanged>> ExecuteAsync(
        UpdateFinOpsReportOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<FinOpsReportOptimizationChanged>.Invalid(issues);
        }

        FinOpsReportOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new FinOpsReportOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<FinOpsReportOptimizationChanged>.Invalid(
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

        FinOpsReportOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<FinOpsReportOptimizationChanged>.Success(changed);
    }
}