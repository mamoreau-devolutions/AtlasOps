namespace AtlasOps.Features.ServiceManagement.ProblemRecordOptimization;

using AtlasOps.Features;

public sealed class ProblemRecordOptimizationService(
    IAtlasOpsCapabilityRepository<ProblemRecordOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ProblemRecordOptimizationValidator validator = new();
    private readonly ProblemRecordOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ProblemRecordOptimizationChanged>> ExecuteAsync(
        UpdateProblemRecordOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ProblemRecordOptimizationChanged>.Invalid(issues);
        }

        ProblemRecordOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ProblemRecordOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ProblemRecordOptimizationChanged>.Invalid(
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

        ProblemRecordOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ProblemRecordOptimizationChanged>.Success(changed);
    }
}