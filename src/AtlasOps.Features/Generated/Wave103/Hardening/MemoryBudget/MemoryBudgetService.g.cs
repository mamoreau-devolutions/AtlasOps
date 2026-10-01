namespace AtlasOps.Features.Hardening.MemoryBudget;

using AtlasOps.Features;

public sealed class MemoryBudgetService(
    IAtlasOpsCapabilityRepository<MemoryBudgetItem> repository,
    TimeProvider timeProvider)
{
    private readonly MemoryBudgetValidator validator = new();
    private readonly MemoryBudgetPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MemoryBudgetChanged>> ExecuteAsync(
        UpdateMemoryBudgetCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MemoryBudgetChanged>.Invalid(issues);
        }

        MemoryBudgetItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MemoryBudgetItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MemoryBudgetChanged>.Invalid(
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

        MemoryBudgetChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MemoryBudgetChanged>.Success(changed);
    }
}