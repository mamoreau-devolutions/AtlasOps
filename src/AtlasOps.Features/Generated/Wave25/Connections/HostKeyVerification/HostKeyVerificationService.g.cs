namespace AtlasOps.Features.Connections.HostKeyVerification;

using AtlasOps.Features;

public sealed class HostKeyVerificationService(
    IAtlasOpsCapabilityRepository<HostKeyVerificationItem> repository,
    TimeProvider timeProvider)
{
    private readonly HostKeyVerificationValidator validator = new();
    private readonly HostKeyVerificationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<HostKeyVerificationChanged>> ExecuteAsync(
        UpdateHostKeyVerificationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<HostKeyVerificationChanged>.Invalid(issues);
        }

        HostKeyVerificationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new HostKeyVerificationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<HostKeyVerificationChanged>.Invalid(
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

        HostKeyVerificationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<HostKeyVerificationChanged>.Success(changed);
    }
}