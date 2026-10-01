namespace AtlasOps.Features.Cloud.AwsAccountOptimization;

using AtlasOps.Features;

public sealed class AwsAccountOptimizationService(
    IAtlasOpsCapabilityRepository<AwsAccountOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly AwsAccountOptimizationValidator validator = new();
    private readonly AwsAccountOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<AwsAccountOptimizationChanged>> ExecuteAsync(
        UpdateAwsAccountOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<AwsAccountOptimizationChanged>.Invalid(issues);
        }

        AwsAccountOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new AwsAccountOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<AwsAccountOptimizationChanged>.Invalid(
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

        AwsAccountOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<AwsAccountOptimizationChanged>.Success(changed);
    }
}