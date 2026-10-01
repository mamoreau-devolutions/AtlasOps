namespace AtlasOps.Features.FinOps.CostCenterRecovery;

using AtlasOps.Features;

public sealed class CostCenterRecoveryService(
    IAtlasOpsCapabilityRepository<CostCenterRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly CostCenterRecoveryValidator validator = new();
    private readonly CostCenterRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CostCenterRecoveryChanged>> ExecuteAsync(
        UpdateCostCenterRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CostCenterRecoveryChanged>.Invalid(issues);
        }

        CostCenterRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CostCenterRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CostCenterRecoveryChanged>.Invalid(
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

        CostCenterRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CostCenterRecoveryChanged>.Success(changed);
    }
}