namespace AtlasOps.Features.Incidents.ErrorBudget;

using AtlasOps.Features;

public sealed class ErrorBudgetService(
    IAtlasOpsCapabilityRepository<ErrorBudgetItem> repository,
    TimeProvider timeProvider)
{
    private readonly ErrorBudgetValidator validator = new();
    private readonly ErrorBudgetPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ErrorBudgetChanged>> ExecuteAsync(
        UpdateErrorBudgetCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ErrorBudgetChanged>.Invalid(issues);
        }

        ErrorBudgetItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ErrorBudgetItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ErrorBudgetChanged>.Invalid(
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

        ErrorBudgetChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ErrorBudgetChanged>.Success(changed);
    }
}