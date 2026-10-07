namespace AtlasOps.Core;

using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

using AtlasOps.Core.Generated;

public interface IAtlasOpsStore
{
    string Kind { get; }

    Task<IReadOnlyList<TModel>> LoadAsync<TModel>(CancellationToken cancellationToken = default)
        where TModel : class, IAtlasOpsEntity;

    Task SaveAsync<TModel>(IEnumerable<TModel> models, CancellationToken cancellationToken = default)
        where TModel : class, IAtlasOpsEntity;
}

public sealed class FileAtlasOpsStore : IAtlasOpsStore
{
    private readonly string directory;

    public FileAtlasOpsStore(string directory)
    {
        this.directory = directory;
        Directory.CreateDirectory(directory);
    }

    public string Kind => "Local JSON";

    public async Task<IReadOnlyList<TModel>> LoadAsync<TModel>(CancellationToken cancellationToken = default)
        where TModel : class, IAtlasOpsEntity
    {
        string path = this.GetPath<TModel>();
        if (!File.Exists(path))
        {
            return [];
        }

        string payload = await File.ReadAllTextAsync(path, cancellationToken);
        return JsonSerializer.Deserialize(payload, AtlasOpsGeneratedSerializer.GetTypeInfo<List<TModel>>()) ?? [];
    }

    public async Task SaveAsync<TModel>(
        IEnumerable<TModel> models,
        CancellationToken cancellationToken = default)
        where TModel : class, IAtlasOpsEntity
    {
        List<TModel> materializedModels = models.ToList();
        string payload = JsonSerializer.Serialize(materializedModels, AtlasOpsGeneratedSerializer.GetTypeInfo<List<TModel>>());
        string path = this.GetPath<TModel>();
        string temporaryPath = path + ".tmp";
        await File.WriteAllTextAsync(temporaryPath, payload, cancellationToken);
        File.Move(temporaryPath, path, overwrite: true);
    }

    private string GetPath<TModel>()
    {
        return Path.Combine(this.directory, $"{typeof(TModel).Name}.json");
    }
}

public sealed class TursoAtlasOpsStore : IAtlasOpsStore
{
    private const string CreateTableSql =
        "CREATE TABLE IF NOT EXISTS atlasops_entities (" +
        "entity_type TEXT NOT NULL, entity_id TEXT NOT NULL, payload TEXT NOT NULL, updated_at TEXT NOT NULL, " +
        "PRIMARY KEY (entity_type, entity_id))";

    private const string LoadSql =
        "SELECT payload FROM atlasops_entities WHERE entity_type = ? ORDER BY updated_at DESC";

    private const string DeleteSql =
        "DELETE FROM atlasops_entities WHERE entity_type = ?";

    private const string UpsertSql =
        "INSERT INTO atlasops_entities (entity_type, entity_id, payload, updated_at) VALUES (?, ?, ?, ?) " +
        "ON CONFLICT(entity_type, entity_id) DO UPDATE SET payload = excluded.payload, updated_at = excluded.updated_at";

    private static readonly HttpClient Client = new();
    private readonly Uri pipelineUri;
    private readonly string token;
    private readonly SemaphoreSlim initializationLock = new(1, 1);
    private bool initialized;

    public TursoAtlasOpsStore(string url, string token)
    {
        this.pipelineUri = CreatePipelineUri(url);
        this.token = string.IsNullOrWhiteSpace(token)
            ? throw new ArgumentException("A Turso authentication token is required.", nameof(token))
            : token;
    }

    public string Kind => "Turso/libSQL";

    public async Task<IReadOnlyList<TModel>> LoadAsync<TModel>(CancellationToken cancellationToken = default)
        where TModel : class, IAtlasOpsEntity
    {
        await this.EnsureInitializedAsync(cancellationToken);
        JsonDocument response = await this.SendAsync(
            CreatePipelineRequest(
                new TursoStatement(LoadSql, [CreateTextArgument(typeof(TModel).Name)])),
            cancellationToken);

        using (response)
        {
            JsonElement rows = response.RootElement
                .GetProperty("results")[0]
                .GetProperty("response")
                .GetProperty("result")
                .GetProperty("rows");

            List<TModel> models = new(rows.GetArrayLength());
            foreach (JsonElement row in rows.EnumerateArray())
            {
                string payload = row[0].GetProperty("value").GetString()
                    ?? throw new JsonException("Turso returned an empty entity payload.");
                models.Add(AtlasOpsGeneratedSerializer.Deserialize<TModel>(payload));
            }

            return models;
        }
    }

    public async Task SaveAsync<TModel>(
        IEnumerable<TModel> models,
        CancellationToken cancellationToken = default)
        where TModel : class, IAtlasOpsEntity
    {
        await this.EnsureInitializedAsync(cancellationToken);
        List<TursoStatement> statements =
        [
            new TursoStatement(DeleteSql, [CreateTextArgument(typeof(TModel).Name)]),
        ];
        statements.AddRange(models.Select(model => new TursoStatement(
                UpsertSql,
                [
                    CreateTextArgument(typeof(TModel).Name),
                    CreateTextArgument(model.Id),
                    CreateTextArgument(AtlasOpsGeneratedSerializer.Serialize(model)),
                    CreateTextArgument(model.UpdatedAt.ToUniversalTime().ToString("O")),
                ])));

        using JsonDocument response = await this.SendAsync(CreatePipelineRequest(statements), cancellationToken);
    }

    private static Uri CreatePipelineUri(string url)
    {
        string normalizedUrl = url.StartsWith("libsql://", StringComparison.OrdinalIgnoreCase)
            ? "https://" + url["libsql://".Length..]
            : url;

        if (!Uri.TryCreate(normalizedUrl, UriKind.Absolute, out Uri? uri) ||
            !string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("The Turso URL must use libsql:// or https://.", nameof(url));
        }

        return new Uri(uri, "/v2/pipeline");
    }

    private static JsonObject CreatePipelineRequest(TursoStatement statement)
    {
        return CreatePipelineRequest([statement]);
    }

    private static JsonObject CreatePipelineRequest(IEnumerable<TursoStatement> statements)
    {
        JsonArray requests = [];
        foreach (TursoStatement statement in statements)
        {
            JsonArray arguments = [];
            foreach (TursoArgument argument in statement.Arguments)
            {
                JsonNode item = new JsonObject
                {
                    ["type"] = argument.Type,
                    ["value"] = argument.Value,
                };
                arguments.Add(item);
            }

            JsonNode request = new JsonObject
            {
                ["type"] = "execute",
                ["stmt"] = new JsonObject
                {
                    ["sql"] = statement.Sql,
                    ["args"] = arguments,
                },
            };
            requests.Add(request);
        }

        return new JsonObject { ["requests"] = requests };
    }

    private static TursoArgument CreateTextArgument(string value)
    {
        return new TursoArgument("text", value);
    }

    private async Task EnsureInitializedAsync(CancellationToken cancellationToken)
    {
        if (this.initialized)
        {
            return;
        }

        await this.initializationLock.WaitAsync(cancellationToken);
        try
        {
            if (this.initialized)
            {
                return;
            }

            using JsonDocument response = await this.SendAsync(
                CreatePipelineRequest(new TursoStatement(CreateTableSql, [])),
                cancellationToken);
            this.initialized = true;
        }
        finally
        {
            this.initializationLock.Release();
        }
    }

    private async Task<JsonDocument> SendAsync(JsonObject payload, CancellationToken cancellationToken)
    {
        using HttpRequestMessage request = new(HttpMethod.Post, this.pipelineUri)
        {
            Content = new StringContent(payload.ToJsonString(), Encoding.UTF8, "application/json"),
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.token);

        using HttpResponseMessage response = await Client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        JsonDocument document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        ValidatePipelineResponse(document);
        return document;
    }

    private static void ValidatePipelineResponse(JsonDocument document)
    {
        JsonElement results = document.RootElement.GetProperty("results");
        foreach (JsonElement result in results.EnumerateArray())
        {
            string type = result.GetProperty("type").GetString() ?? string.Empty;
            if (!string.Equals(type, "ok", StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Turso rejected an AtlasOps persistence operation.");
            }
        }
    }

    private sealed record TursoStatement(string Sql, IReadOnlyList<TursoArgument> Arguments);

    private sealed record TursoArgument(string Type, string Value);
}

public static class AtlasOpsStoreFactory
{
    public static IAtlasOpsStore Create(AtlasOpsSettings settings)
    {
        if (settings.UseTurso)
        {
            return new TursoAtlasOpsStore(settings.TursoUrl, settings.TursoToken);
        }

        return new FileAtlasOpsStore(settings.LocalDataPath);
    }
}
