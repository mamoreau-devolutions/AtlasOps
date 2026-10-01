namespace AtlasOps.Features.Api.ApiClientRecovery;

using AtlasOps.Features;

public sealed class ApiClientRecoveryService(
    IAtlasOpsCapabilityRepository<ApiClientRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly ApiClientRecoveryValidator validator = new();
    private readonly ApiClientRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ApiClientRecoveryChanged>> ExecuteAsync(
        UpdateApiClientRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ApiClientRecoveryChanged>.Invalid(issues);
        }

        ApiClientRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ApiClientRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ApiClientRecoveryChanged>.Invalid(
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

        ApiClientRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ApiClientRecoveryChanged>.Success(changed);
    }
}