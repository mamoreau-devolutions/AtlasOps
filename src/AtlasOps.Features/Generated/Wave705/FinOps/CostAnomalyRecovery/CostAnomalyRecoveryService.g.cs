namespace AtlasOps.Features.FinOps.CostAnomalyRecovery;

using AtlasOps.Features;

public sealed class CostAnomalyRecoveryService(
    IAtlasOpsCapabilityRepository<CostAnomalyRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly CostAnomalyRecoveryValidator validator = new();
    private readonly CostAnomalyRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CostAnomalyRecoveryChanged>> ExecuteAsync(
        UpdateCostAnomalyRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CostAnomalyRecoveryChanged>.Invalid(issues);
        }

        CostAnomalyRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CostAnomalyRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CostAnomalyRecoveryChanged>.Invalid(
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

        CostAnomalyRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CostAnomalyRecoveryChanged>.Success(changed);
    }
}