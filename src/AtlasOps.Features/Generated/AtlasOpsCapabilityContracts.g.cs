namespace AtlasOps.Features;

public sealed record AtlasOpsCapabilityDescriptor(
    string Id,
    string DisplayName,
    string Area,
    int Wave,
    Type ModelType,
    Type ViewModelType,
    Type ViewType,
    Func<object> CreateModel,
    Func<object> CreateViewModel,
    Func<object> CreateView);

public sealed record AtlasOpsValidationIssue(string Field, string Message);

public sealed record AtlasOpsOperationResult<T>(bool IsSuccess, T? Value, IReadOnlyList<AtlasOpsValidationIssue> Issues)
{
    public static AtlasOpsOperationResult<T> Success(T value) => new(true, value, []);

    public static AtlasOpsOperationResult<T> Invalid(IReadOnlyList<AtlasOpsValidationIssue> issues) => new(false, default, issues);
}

public interface IAtlasOpsCapabilityEntity
{
    string Id { get; }
    string Name { get; set; }
    string Owner { get; set; }
    string State { get; set; }
    int Priority { get; set; }
    bool IsEnabled { get; set; }
    DateTimeOffset UpdatedAt { get; set; }
}

public interface IAtlasOpsCapabilityRepository<T>
    where T : class, IAtlasOpsCapabilityEntity
{
    Task<T?> GetAsync(string id, CancellationToken cancellationToken);
    Task SaveAsync(T entity, CancellationToken cancellationToken);
    Task<IReadOnlyList<T>> ListAsync(CancellationToken cancellationToken);
}

public sealed class InMemoryAtlasOpsCapabilityRepository<T> : IAtlasOpsCapabilityRepository<T>
    where T : class, IAtlasOpsCapabilityEntity
{
    private readonly Dictionary<string, T> entities = new(StringComparer.Ordinal);

    public Task<T?> GetAsync(string id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        this.entities.TryGetValue(id, out T? entity);
        return Task.FromResult(entity);
    }

    public Task SaveAsync(T entity, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        this.entities[entity.Id] = entity;
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<T>> ListAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyList<T> result = this.entities.Values.OrderBy(static entity => entity.Name, StringComparer.OrdinalIgnoreCase).ToArray();
        return Task.FromResult(result);
    }
}