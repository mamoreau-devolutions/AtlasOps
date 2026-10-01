namespace AtlasOps.Features.Api.ApiQuotaRecovery;

using AtlasOps.Features;

public sealed class ApiQuotaRecoveryService(
    IAtlasOpsCapabilityRepository<ApiQuotaRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly ApiQuotaRecoveryValidator validator = new();
    private readonly ApiQuotaRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ApiQuotaRecoveryChanged>> ExecuteAsync(
        UpdateApiQuotaRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ApiQuotaRecoveryChanged>.Invalid(issues);
        }

        ApiQuotaRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ApiQuotaRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ApiQuotaRecoveryChanged>.Invalid(
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

        ApiQuotaRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ApiQuotaRecoveryChanged>.Success(changed);
    }
}