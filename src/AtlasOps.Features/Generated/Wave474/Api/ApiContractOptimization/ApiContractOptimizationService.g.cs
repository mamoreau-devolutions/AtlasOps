namespace AtlasOps.Features.Api.ApiContractOptimization;

using AtlasOps.Features;

public sealed class ApiContractOptimizationService(
    IAtlasOpsCapabilityRepository<ApiContractOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ApiContractOptimizationValidator validator = new();
    private readonly ApiContractOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ApiContractOptimizationChanged>> ExecuteAsync(
        UpdateApiContractOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ApiContractOptimizationChanged>.Invalid(issues);
        }

        ApiContractOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ApiContractOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ApiContractOptimizationChanged>.Invalid(
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

        ApiContractOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ApiContractOptimizationChanged>.Success(changed);
    }
}