namespace AtlasOps.Features.Data.DataContractOptimization;

using AtlasOps.Features;

public sealed class DataContractOptimizationService(
    IAtlasOpsCapabilityRepository<DataContractOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly DataContractOptimizationValidator validator = new();
    private readonly DataContractOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DataContractOptimizationChanged>> ExecuteAsync(
        UpdateDataContractOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DataContractOptimizationChanged>.Invalid(issues);
        }

        DataContractOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DataContractOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DataContractOptimizationChanged>.Invalid(
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

        DataContractOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DataContractOptimizationChanged>.Success(changed);
    }
}