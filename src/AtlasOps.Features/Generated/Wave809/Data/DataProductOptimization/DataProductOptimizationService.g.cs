namespace AtlasOps.Features.Data.DataProductOptimization;

using AtlasOps.Features;

public sealed class DataProductOptimizationService(
    IAtlasOpsCapabilityRepository<DataProductOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly DataProductOptimizationValidator validator = new();
    private readonly DataProductOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DataProductOptimizationChanged>> ExecuteAsync(
        UpdateDataProductOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DataProductOptimizationChanged>.Invalid(issues);
        }

        DataProductOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DataProductOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DataProductOptimizationChanged>.Invalid(
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

        DataProductOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DataProductOptimizationChanged>.Success(changed);
    }
}