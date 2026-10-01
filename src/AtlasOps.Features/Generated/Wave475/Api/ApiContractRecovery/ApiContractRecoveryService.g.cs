namespace AtlasOps.Features.Api.ApiContractRecovery;

using AtlasOps.Features;

public sealed class ApiContractRecoveryService(
    IAtlasOpsCapabilityRepository<ApiContractRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly ApiContractRecoveryValidator validator = new();
    private readonly ApiContractRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ApiContractRecoveryChanged>> ExecuteAsync(
        UpdateApiContractRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ApiContractRecoveryChanged>.Invalid(issues);
        }

        ApiContractRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ApiContractRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ApiContractRecoveryChanged>.Invalid(
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

        ApiContractRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ApiContractRecoveryChanged>.Success(changed);
    }
}