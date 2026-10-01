namespace AtlasOps.Features.ServiceManagement.ServiceReviewRecovery;

using AtlasOps.Features;

public sealed class ServiceReviewRecoveryService(
    IAtlasOpsCapabilityRepository<ServiceReviewRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly ServiceReviewRecoveryValidator validator = new();
    private readonly ServiceReviewRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ServiceReviewRecoveryChanged>> ExecuteAsync(
        UpdateServiceReviewRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ServiceReviewRecoveryChanged>.Invalid(issues);
        }

        ServiceReviewRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ServiceReviewRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ServiceReviewRecoveryChanged>.Invalid(
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

        ServiceReviewRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ServiceReviewRecoveryChanged>.Success(changed);
    }
}