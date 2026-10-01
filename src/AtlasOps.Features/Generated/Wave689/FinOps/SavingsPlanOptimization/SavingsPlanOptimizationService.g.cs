namespace AtlasOps.Features.FinOps.SavingsPlanOptimization;

using AtlasOps.Features;

public sealed class SavingsPlanOptimizationService(
    IAtlasOpsCapabilityRepository<SavingsPlanOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly SavingsPlanOptimizationValidator validator = new();
    private readonly SavingsPlanOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SavingsPlanOptimizationChanged>> ExecuteAsync(
        UpdateSavingsPlanOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SavingsPlanOptimizationChanged>.Invalid(issues);
        }

        SavingsPlanOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SavingsPlanOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SavingsPlanOptimizationChanged>.Invalid(
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

        SavingsPlanOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SavingsPlanOptimizationChanged>.Success(changed);
    }
}