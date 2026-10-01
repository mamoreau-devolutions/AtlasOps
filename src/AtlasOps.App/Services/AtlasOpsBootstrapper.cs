namespace AtlasOps.App.Services;

using AtlasOps.Core;
using AtlasOps.Core.Generated;

public sealed class AtlasOpsBootstrapper
{
    private readonly IAtlasOpsStore store;

    public AtlasOpsBootstrapper(string? localDataPath = null, bool forceLocal = false)
    {
        string dataPath = localDataPath ??
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AtlasOps");
        string tursoUrl = Environment.GetEnvironmentVariable("TURSO_DATABASE_URL") ?? string.Empty;
        string tursoToken = Environment.GetEnvironmentVariable("TURSO_AUTH_TOKEN") ?? string.Empty;
        bool useTurso = !forceLocal &&
            !string.IsNullOrWhiteSpace(tursoUrl) &&
            !string.IsNullOrWhiteSpace(tursoToken);

        this.Settings = new AtlasOpsSettings
        {
            WorkspaceName = "AtlasOps Operations Workspace",
            LocalDataPath = dataPath,
            TursoUrl = tursoUrl,
            TursoToken = tursoToken,
            UseTurso = useTurso,
        };
        this.SettingsService = new AtlasOpsSettingsService(dataPath);
        this.store = AtlasOpsStoreFactory.Create(this.Settings);
    }

    public AtlasOpsSettings Settings { get; }

    public IAtlasOpsSettingsService SettingsService { get; }

    public string PersistenceKind => this.store.Kind;

    public Task<AtlasOpsLayoutState> LoadLayoutAsync(CancellationToken cancellationToken = default)
    {
        return this.SettingsService.LoadLayoutAsync(cancellationToken);
    }

    public Task SaveLayoutAsync(AtlasOpsLayoutState layout, CancellationToken cancellationToken = default)
    {
        return this.SettingsService.SaveLayoutAsync(layout, cancellationToken);
    }

    public async Task<AtlasOpsGeneratedWorkspace> LoadWorkspaceAsync(CancellationToken cancellationToken = default)
    {
        AtlasOpsGeneratedWorkspace workspace = await AtlasOpsGeneratedWorkspaceManager.LoadAsync(this.store, cancellationToken);
        if (workspace.Entities.Count == 0)
        {
            AtlasOpsGeneratedWorkspace seed = CreateSeedWorkspace();
            await this.SaveWorkspaceAsync(seed, cancellationToken);
            return seed;
        }

        return workspace;
    }

    public async Task SaveWorkspaceAsync(
        AtlasOpsGeneratedWorkspace workspace,
        CancellationToken cancellationToken = default)
    {
        await AtlasOpsGeneratedWorkspaceManager.SaveAsync(this.store, workspace.Entities, cancellationToken);
    }

    private static AtlasOpsGeneratedWorkspace CreateSeedWorkspace()
    {
        List<IAtlasOpsEntity> entities =
        [
            new AtlasOpsProject { Name = "AtlasOps launch", Stage = "Prototype", Owner = "Platform", Priority = 1, IsPinned = true },
            new AtlasOpsProject { Name = "Fleet migration", Stage = "Architecture", Owner = "Desktop", Priority = 2 },
            new AtlasOpsProject { Name = "Vault sync hardening", Stage = "Operations", Owner = "Security", Priority = 2 },
            new AtlasOpsConnection { Name = "Turso production", Host = "atlasops.turso.io", Database = "atlasops", Protocol = "libSQL", LastStatus = "Connected" },
            new AtlasOpsConnection { Name = "Lab jump host", Host = "10.20.0.15", Database = "operations", Protocol = "SSH", LastStatus = "Ready" },
            new AtlasOpsConnection { Name = "Windows build host", Host = "build-win-04", Database = "operations", Protocol = "RDP", LastStatus = "Ready" },
            new AtlasOpsTaskItem { Title = "Validate generated model editors", Status = "In progress", Assignee = "Platform", Completion = 70, IsHighPriority = true },
            new AtlasOpsTaskItem { Title = "Review deployment runbooks", Status = "To Do", Assignee = "Operations", Completion = 15 },
            new AtlasOpsTaskItem { Title = "Publish workspace baseline", Status = "To Do", Assignee = "Desktop", Completion = 35 },
            new AtlasOpsEditorDocument { Title = "Launch brief", Language = "markdown", Content = "# AtlasOps launch plan" },
            new AtlasOpsEditorDocument { Title = "Connection catalog", Language = "yaml", Content = "database: atlasops" },
            new AtlasOpsEditorDocument { Title = "Health query", Language = "sql", Content = "SELECT * FROM atlasops_entities" },
            new AtlasOpsActivity { Title = "Workspace generated", Kind = "Code generation", Actor = "AtlasOps", Description = "20 domain models and editor sets generated." },
            new AtlasOpsActivity { Title = "Persistence initialized", Kind = "Storage", Actor = "AtlasOps", Description = "Workspace store is ready." },
            new AtlasOpsCredential { Name = "Production operator", UserName = "atlasops", Provider = "Environment", SecretReference = "TURSO_AUTH_TOKEN", IsManaged = true },
            new AtlasOpsEnvironment { Name = "Production", Region = "ca-central-1", Tier = "Production", Health = "Healthy", Endpoint = "https://atlasops.example", IsProduction = true },
            new AtlasOpsHost { Name = "build-win-04", Address = "10.20.1.24", OperatingSystem = "Windows Server 2025", Status = "Online", CpuLoad = 23.5 },
            new AtlasOpsService { Name = "Workspace API", Kind = "ASP.NET Core", Version = "1.0.0", Status = "Running", Replicas = 3, Endpoint = "https://api.atlasops.example" },
            new AtlasOpsDeployment { Name = "AtlasOps 1.0", EnvironmentName = "Production", Version = "1.0.0", Status = "Succeeded", RequestedBy = "Platform", Progress = 100 },
            new AtlasOpsRunbook { Title = "Recover workspace API", Category = "Recovery", Content = "Validate replicas and restart the failed instance.", LastExecutedBy = "Operations", ExecutionCount = 4, IsApproved = true },
            new AtlasOpsIncident { Title = "Elevated API latency", Severity = "Medium", Status = "Monitoring", Commander = "Operations", Summary = "Latency returned to normal after scaling.", AffectedServices = 1 },
            new AtlasOpsAlert { Title = "Build host disk usage", Source = "build-win-04", Severity = "Warning", Status = "Active", Message = "Disk usage is above 80 percent.", IsAcknowledged = false },
            new AtlasOpsMilestone { Name = "Public preview", Status = "In progress", Owner = "Product", Completion = 65, IsBlocked = false },
            new AtlasOpsQuery { Name = "Active incidents", Sql = "SELECT * FROM incidents WHERE status != 'Closed'", ConnectionName = "Turso production", LastDurationMs = 18, LastRowCount = 1, IsFavorite = true },
            new AtlasOpsDashboard { Name = "Operations overview", Layout = "Grid", Owner = "Operations", RefreshSeconds = 30, WidgetCount = 6, IsShared = true },
            new AtlasOpsDashboardCard { Title = "Healthy services", Value = "12/12", Trend = "+1", Kind = "Success", OrderIndex = 1, QueryName = "Service health" },
            new AtlasOpsAuditEvent { Action = "Workspace saved", Actor = "Platform", Target = "AtlasOps", Result = "Success", Details = "Generated workspace persisted.", OccurredAt = DateTimeOffset.UtcNow },
            new AtlasOpsPolicy { Name = "Production approvals", Scope = "Production", Rule = "Require approved deployment", Enforcement = "Block", Priority = 1, IsEnabled = true },
            new AtlasOpsTeam { Name = "Platform", Lead = "Alex", Description = "Owns AtlasOps foundations.", MemberCount = 8, OnCallAlias = "platform-oncall", IsActive = true },
        ];

        return new AtlasOpsGeneratedWorkspace(entities);
    }
}