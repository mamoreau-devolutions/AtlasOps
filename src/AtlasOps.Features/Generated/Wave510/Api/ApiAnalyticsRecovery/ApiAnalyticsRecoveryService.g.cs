namespace AtlasOps.Features.Api.ApiAnalyticsRecovery;

using AtlasOps.Features;

public sealed class ApiAnalyticsRecoveryService(
    IAtlasOpsCapabilityRepository<ApiAnalyticsRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly ApiAnalyticsRecoveryValidator validator = new();
    private readonly ApiAnalyticsRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ApiAnalyticsRecoveryChanged>> ExecuteAsync(
        UpdateApiAnalyticsRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ApiAnalyticsRecoveryChanged>.Invalid(issues);
        }

        ApiAnalyticsRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ApiAnalyticsRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ApiAnalyticsRecoveryChanged>.Invalid(
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

        ApiAnalyticsRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ApiAnalyticsRecoveryChanged>.Success(changed);
    }
}