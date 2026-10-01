namespace AtlasOps.Features.Api.ApiEndpointRecovery;

using AtlasOps.Features;

public sealed class ApiEndpointRecoveryService(
    IAtlasOpsCapabilityRepository<ApiEndpointRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly ApiEndpointRecoveryValidator validator = new();
    private readonly ApiEndpointRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ApiEndpointRecoveryChanged>> ExecuteAsync(
        UpdateApiEndpointRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ApiEndpointRecoveryChanged>.Invalid(issues);
        }

        ApiEndpointRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ApiEndpointRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ApiEndpointRecoveryChanged>.Invalid(
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

        ApiEndpointRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ApiEndpointRecoveryChanged>.Success(changed);
    }
}