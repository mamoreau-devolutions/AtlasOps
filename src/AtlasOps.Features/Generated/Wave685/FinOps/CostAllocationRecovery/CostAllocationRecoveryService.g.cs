namespace AtlasOps.Features.FinOps.CostAllocationRecovery;

using AtlasOps.Features;

public sealed class CostAllocationRecoveryService(
    IAtlasOpsCapabilityRepository<CostAllocationRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly CostAllocationRecoveryValidator validator = new();
    private readonly CostAllocationRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CostAllocationRecoveryChanged>> ExecuteAsync(
        UpdateCostAllocationRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CostAllocationRecoveryChanged>.Invalid(issues);
        }

        CostAllocationRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CostAllocationRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CostAllocationRecoveryChanged>.Invalid(
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

        CostAllocationRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CostAllocationRecoveryChanged>.Success(changed);
    }
}