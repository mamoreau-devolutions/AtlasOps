namespace AtlasOps.Features.Connections.HttpEndpoint;

using AtlasOps.Features;

public sealed class HttpEndpointService(
    IAtlasOpsCapabilityRepository<HttpEndpointItem> repository,
    TimeProvider timeProvider)
{
    private readonly HttpEndpointValidator validator = new();
    private readonly HttpEndpointPolicy policy = new();

    public async Task<AtlasOpsOperationResult<HttpEndpointChanged>> ExecuteAsync(
        UpdateHttpEndpointCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<HttpEndpointChanged>.Invalid(issues);
        }

        HttpEndpointItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new HttpEndpointItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<HttpEndpointChanged>.Invalid(
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

        HttpEndpointChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<HttpEndpointChanged>.Success(changed);
    }
}