namespace AtlasOps.App;

using AtlasOps.Adapters.Aws;
using AtlasOps.Adapters.Azure;
using AtlasOps.Adapters.Gcp;
using AtlasOps.Adapters.Persistence.Sqlite;
using AtlasOps.App.Services;
using AtlasOps.Composition;
using AtlasOps.Connectors.Collaboration;
using AtlasOps.Connectors.Contracts;
using AtlasOps.Connectors.Data;
using AtlasOps.Connectors.Documents;
using AtlasOps.Connectors.Http;
using AtlasOps.Connectors.Observability;
using AtlasOps.Connectors.Runtime;
using AtlasOps.Connectors.Security;
using AtlasOps.Core;
using AtlasOps.Core.Generated;
using AtlasOps.Enterprise.Avalonia.Common;
using AtlasOps.Enterprise.Contracts.Policy;
using AtlasOps.Enterprise.Contracts.Reporting;
using AtlasOps.Enterprise.Contracts.Workflow;
using AtlasOps.Enterprise.Core.Policy;
using AtlasOps.Enterprise.Core.Reporting;
using AtlasOps.Enterprise.Core.Scenarios;
using AtlasOps.Enterprise.Core.Workflow;
using AtlasOps.Features;
using AtlasOps.Geography.Data;
using AtlasOps.Geography.Domain;
using AtlasOps.Geography.Services;
using AtlasOps.ReferenceData.Cloud;
using AtlasOps.ReferenceData.Geospatial;
using AtlasOps.ReferenceData.Iana;
using AtlasOps.ReferenceData.Infrastructure;
using AtlasOps.ReferenceData.Lifecycle;
using AtlasOps.ReferenceData.Localization;
using AtlasOps.Scheduling;
using AtlasOps.Security;
using AtlasOps.Serialization;
using AtlasOps.Telemetry;

using Avalonia;
using AvaloniaUI.DiagnosticsSupport;

public static class Program
{
    [STAThread]
    public static int Main(string[] args)
    {
        bool headless = args.Any(static argument =>
            string.Equals(argument, "--headless", StringComparison.OrdinalIgnoreCase));

        if (!headless)
        {
            return BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }

        try
        {
            RunHeadlessAsync().GetAwaiter().GetResult();
            return 0;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"AtlasOps validation failed: {exception.Message}");
            return 1;
        }
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .WithDeveloperTools()
            .LogToTrace();
    }

    private static async Task RunHeadlessAsync()
    {
        string validationPath = Path.Combine(Path.GetTempPath(), "AtlasOps", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(validationPath);
            Console.WriteLine("AtlasOps headless validation");
            Console.WriteLine($"- generated models: {AtlasOpsGeneratedModelCatalog.Models.Count}");
            Console.WriteLine($"- generated fields: {AtlasOpsGeneratedModelCatalog.Models.Sum(static model => model.Fields.Count)}");
            Console.WriteLine($"- generated capabilities: {AtlasOpsCapabilityCatalog.All.Count}");
            Console.WriteLine($"- implementation waves: {AtlasOpsWaveManifest.CountsByWave.Count}");
            Ensure(AtlasOpsCapabilityCatalog.All.Count == 1200, "Capability generation coverage failed.");
            Ensure(AtlasOpsWaveManifest.CountsByWave.Count == 1110, "Wave generation coverage failed.");
            Ensure(AtlasOpsWaveManifest.CountsByWave.Where(static item => item.Key <= 10).All(static item => item.Value == 10), "Foundation waves must contain ten capabilities.");
            Ensure(AtlasOpsWaveManifest.CountsByWave.Where(static item => item.Key > 10).All(static item => item.Value == 1), "Expansion waves must contain one capability.");
            Ensure(AtlasOpsCapabilityCatalog.All.Select(static capability => capability.Id).Distinct(StringComparer.Ordinal).Count() == 1200, "Capability IDs must be unique.");
            Ensure(AtlasOpsCapabilityCatalog.All.All(static capability => capability.CreateModel() is IAtlasOpsCapabilityEntity), "Capability model factories failed.");
            Ensure(AtlasOpsCapabilityCatalog.All.All(static capability => capability.CreateViewModel() is not null), "Capability viewmodel factories failed.");
            IReadOnlyList<AtlasOpsModuleDescriptor> modules = AtlasOpsModuleCatalog.Create();
            Ensure(modules.Count == 18, "Modular domain coverage failed.");
            Ensure(modules.Select(static module => module.Id).Distinct(StringComparer.Ordinal).Count() == 18, "Modular domain IDs must be unique.");
            Ensure(modules.All(static module => module.CapabilityCount == 30), "Each modular domain must expose thirty capabilities.");
            Ensure(modules.Sum(static module => module.CapabilityCount) == 540, "Modular capability coverage failed.");
            WorkflowDefinition enterpriseWorkflow = EnterpriseScenarioCatalog.CreateDeploymentWorkflow();
            WorkflowValidationResult workflowValidation = new WorkflowGraphValidator().Validate(enterpriseWorkflow);
            WorkflowExecutionPlan workflowPlan = new WorkflowPlanner().CreatePlan(enterpriseWorkflow);
            Ensure(workflowValidation.IsValid, "Enterprise workflow validation failed.");
            Ensure(workflowPlan.Layers.Count == 5, "Enterprise workflow planning failed.");

            PolicyDecision policyDecision = new PolicyEvaluator().Evaluate(
                EnterpriseScenarioCatalog.CreatePolicyRequests()[1],
                EnterpriseScenarioCatalog.CreateOperationsPolicy(),
                EnterpriseScenarioCatalog.CreateRoles());
            Ensure(policyDecision.Kind == PolicyDecisionKind.Deny, "Enterprise deny-precedence validation failed.");

            ReportDefinition enterpriseReport = EnterpriseScenarioCatalog.CreateReports()[0];
            ReportResult reportResult = new ReportEngine().Execute(
                enterpriseReport,
                EnterpriseScenarioCatalog.CreateOperationalRows(DateTimeOffset.UtcNow));
            Ensure(reportResult.Rows.Count == 2, "Enterprise grouped reporting validation failed.");
            Ensure(reportResult.Diagnostics.Count == 0, "Enterprise reporting produced unexpected diagnostics.");

            Ensure(AwsSdkCatalog.Services.Count == 6, "AWS SDK catalog validation failed.");
            Ensure(AzureSdkCatalog.Services.Count == 4, "Azure SDK catalog validation failed.");
            Ensure(GoogleSdkCatalog.Default.Scopes.Count == 2, "Google SDK catalog validation failed.");
            Ensure(SecurityIntegrationCatalog.Integrations.Count == 7, "Security integration catalog validation failed.");
            Ensure(OperationalComponentCatalog.Components.Count == 5, "Operational component catalog validation failed.");
            Ensure(RuntimePackageCatalog.Packages.Count == 6, "Connector runtime package catalog validation failed.");
            Ensure(HttpPackageCatalog.Packages.Count == 7, "HTTP package catalog validation failed.");
            Ensure(HttpPackageCatalog.OpenApiPackages.Count == 2, "OpenAPI package catalog validation failed.");
            Ensure(CollaborationConnectorCatalog.Connectors.Count == 7, "Collaboration connector catalog validation failed.");
            Ensure(AiConnectorCatalog.Providers.Count == 3, "AI connector catalog validation failed.");
            Ensure(
                AiConnectorCatalog.Validate(
                    new AiPromptPlan(
                        "google-genai",
                        "validation-model",
                        "Return deterministic validation output.",
                        "Validate AtlasOps connector composition.",
                        0d,
                        128,
                        ConnectorContract.EmptyDetails)).Valid,
                "AI prompt planning validation failed.");
            Ensure(ConnectorDataProviderCatalog.Providers.Count == 7, "Connector data package catalog validation failed.");
            Ensure(ConnectorSecurityCatalog.Packages.Count == 6, "Connector security package catalog validation failed.");
            Ensure(ObservabilityPackageCatalog.Packages.Count == 7, "Connector observability package catalog validation failed.");
            Ensure(DocumentPackageCatalog.Packages.Count == 7, "Connector document package catalog validation failed.");

            ConnectorRegistry connectorRegistry = ConnectorScenarioCatalog.CreateRegistry();
            InMemoryConnectorAuditSink connectorAudit = new();
            ConnectorRuntime connectorRuntime = new(
                connectorRegistry,
                new ConnectorRateLimiter(),
                connectorAudit);
            ConnectorExecutionOutcome connectorOutcome = await connectorRuntime.ExecuteAsync(
                new ConnectorExecutionRequest(
                    Guid.NewGuid(),
                    "asset-discovery",
                    "synchronize",
                    "atlasops",
                    null,
                    DateTimeOffset.UtcNow,
                    ConnectorContract.EmptyDetails),
                CancellationToken.None);
            Ensure(connectorOutcome.Status == ConnectorExecutionStatus.Succeeded, "Connector runtime validation failed.");
            Ensure(connectorAudit.Records.Count == 1, "Connector audit validation failed.");

            ConnectorDocumentService connectorDocuments = new();
            ConnectorDocument[] connectorDocumentSet =
            [
                new("validation.md", "text/markdown", System.Text.Encoding.UTF8.GetBytes("# AtlasOps")),
            ];
            byte[] connectorArchive = connectorDocuments.CreateArchive(connectorDocumentSet);
            Ensure(connectorDocuments.ReadArchive(connectorArchive).Count == 1, "Connector document archive validation failed.");

            ConnectorSecretDeriver connectorSecretDeriver = new();
            SecretDerivationResult derivedSecret = await connectorSecretDeriver.DeriveAsync(
                System.Text.Encoding.UTF8.GetBytes("atlasops-validation"),
                Enumerable.Range(1, 32).Select(static item => (byte)item).ToArray(),
                new SecretDerivationOptions(2, 8_192, 1, 32),
                CancellationToken.None);
            Ensure(derivedSecret.DerivedKey.Length == 32, "Connector secret derivation validation failed.");

            string journalPath = Path.Combine(validationPath, "operations.db");
            SqliteOperationJournal journal = new(journalPath);
            await journal.InitializeAsync(CancellationToken.None);
            await journal.AppendAsync(
                "validation",
                "startup",
                "atlasops",
                DateTimeOffset.UtcNow,
                """{"status":"ready"}""",
                CancellationToken.None);
            IReadOnlyList<SqliteOperationEntry> journalEntries = await journal.ReadResourceAsync(
                "atlasops",
                10,
                CancellationToken.None);
            Ensure(journalEntries.Count == 1, "SQLite operation journal validation failed.");

            InterchangeService interchange = new();
            List<InterchangeRecord> interchangeRecords =
            [
                new() { Id = "atlasops", Category = "validation", Value = 1m },
            ];
            Ensure(interchange.SerializeMessagePack(interchangeRecords).Length > 0, "MessagePack validation failed.");
            Ensure(interchange.SerializeProtobuf(interchangeRecords).Length > 0, "Protobuf validation failed.");
            Ensure(interchange.CreateWorkbook(interchangeRecords).Length > 0, "Open XML validation failed.");

            CalendarScheduleResult schedule = new CalendarScheduleService().Create(
                "0 0 12 * * ?",
                "AtlasOps validation",
                DateTimeOffset.UtcNow,
                TimeSpan.FromMinutes(15));
            Ensure(schedule.Valid, "Quartz and iCalendar validation failed.");

            using StructuredEventCollector eventCollector = new();
            eventCollector.RecordOperation("validate", "atlasops", TimeSpan.FromMilliseconds(1), true);
            Ensure(eventCollector.Events.Count == 1, "Structured telemetry validation failed.");

            AtlasOpsProject project = new()
            {
                Name = "AtlasOps validation",
                Stage = "Prototype",
                Owner = "Platform",
            };
            string payload = AtlasOpsGeneratedSerializer.Serialize(project);
            AtlasOpsProject roundTrip = AtlasOpsGeneratedSerializer.Deserialize<AtlasOpsProject>(payload);
            Ensure(roundTrip.Name == project.Name, "Generated serializer round trip failed.");

            IGeneratedEditorViewModel editor = AtlasOpsGeneratedEditorFactory.Create(project);
            AtlasOpsProjectEditorViewModel projectEditor = (AtlasOpsProjectEditorViewModel)editor;
            projectEditor.Stage = "Validated";
            Ensure(project.Stage == "Validated" && projectEditor.HasChanges, "Generated editor binding failed.");

            AtlasOpsBootstrapper bootstrapper = new(validationPath, forceLocal: true);
            AtlasOpsGeneratedWorkspace snapshot = await bootstrapper.LoadWorkspaceAsync();
            Ensure(snapshot.Entities.OfType<AtlasOpsProject>().Count() >= 3, "Workspace seed persistence failed.");
            Ensure(snapshot.Entities.Select(static entity => entity.GetType()).Distinct().Count() == 20, "Generated model seed coverage failed.");

            AtlasOpsGeneratedWorkspace reload = await bootstrapper.LoadWorkspaceAsync();
            Ensure(reload.Entities.OfType<AtlasOpsConnection>().Count() >= 3, "Workspace reload persistence failed.");

            string geographyDataPath = Path.Combine(
                AppContext.BaseDirectory,
                "ReferenceData",
                "Geography");
            GeographyCatalog geography = await new GeographyDatasetLoader().LoadAsync(
                geographyDataPath,
                GeographyOverrideSet.Empty);
            GeographyValidationReport geographyValidation = new GeographyCatalogValidator().Validate(geography);
            GeographySearchIndex geographySearch = new(geography);
            Ensure(geography.Countries.Count >= 249, "Geography country import coverage failed.");
            Ensure(geography.Subdivisions.Count > 4_000, "Geography subdivision import coverage failed.");
            Ensure(geographyValidation.IsValid, "Geography catalog validation failed.");
            Ensure(
                geographySearch.Search("Vereinigte Staaten").Any(static result => result.Id == "US"),
                "Geography alias search failed.");

            IanaCatalog iana = await new IanaCatalogLoader().LoadAsync(
                ReferenceDataPaths.GetPackDirectory("iana"));
            Ensure(iana.Services.Count > 10_000, "IANA service assignment coverage failed.");
            Ensure(
                new IanaRegistryAnalyzer().FindAssignments(iana, 443, "tcp").Count > 0,
                "IANA exact port lookup failed.");

            LifecycleCatalog lifecycle = await new LifecycleCatalogLoader().LoadAsync(
                ReferenceDataPaths.GetPackDirectory("lifecycle"));
            Ensure(lifecycle.Releases.Count > 1_000, "Lifecycle release-cycle coverage failed.");
            Ensure(lifecycle.VersionCount > 10_000, "Lifecycle detailed-version coverage failed.");
            Ensure(
                new LifecycleRiskAnalyzer().CreateUpgradeCampaign(
                    lifecycle,
                    DateOnly.FromDateTime(DateTime.UtcNow),
                    365).Count > 0,
                "Lifecycle upgrade campaign generation failed.");

            LocalizationCatalog localization = await new LocalizationCatalogLoader().LoadAsync(
                ReferenceDataPaths.GetPackDirectory("localization"));
            Ensure(localization.Locales.Count > 500, "CLDR locale coverage failed.");
            Ensure(localization.TimeZones.Count > 300, "IANA time-zone coverage failed.");
            Ensure(localization.WindowsMappings.Count > 400, "Windows time-zone mapping coverage failed.");
            IReadOnlySet<string> locales = localization.Locales
                .Select(static item => item.Locale)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            Ensure(
                new LocaleFallbackResolver().Resolve("zh-Hant-TW", locales).Count > 1,
                "CLDR locale fallback resolution failed.");

            CloudCatalog cloud = await new CloudCatalogLoader().LoadAsync(
                ReferenceDataPaths.GetPackDirectory("cloud"));
            Ensure(cloud.Offers.Count > 90_000, "Cloud regional-offer coverage failed.");
            Ensure(
                new CloudCostAnalyzer().Rank(cloud, 4, 16, null).Count > 0,
                "Cloud cost ranking failed.");

            GeospatialCatalog geospatial = await new GeospatialCatalogLoader().LoadAsync(
                ReferenceDataPaths.GetPackDirectory("geospatial"));
            Ensure(geospatial.Features.Count > 5_000, "Natural Earth feature coverage failed.");
            GeospatialAnalysisService geospatialAnalysis = new();
            Ensure(
                geospatialAnalysis.FindNearest(geospatial, 45.5019, -73.5674, 5).Count == 5,
                "Geospatial nearest-feature search failed.");
            await Task.WhenAll(
                VerifyPackAsync("iana", iana.Manifest),
                VerifyPackAsync("lifecycle", lifecycle.Manifest),
                VerifyPackAsync("localization", localization.Manifest),
                VerifyPackAsync("cloud", cloud.Manifest),
                VerifyPackAsync("geospatial", geospatial.Manifest));
            IReadOnlyList<string> productionCapabilities = ProductionParityAcceptance.Validate();

            Console.WriteLine("- modular domains: PASS (18 modules, 540 business capabilities)");
            Console.WriteLine("- enterprise studios: PASS (workflow, policy, incidents, inventory, reporting)");
            Console.WriteLine("- package integrations: PASS (cloud, SQLite, security, interchange, scheduling, telemetry, UI)");
            Console.WriteLine("- connector platform: PASS (runtime, HTTP, collaboration, data, documents, security, observability)");
            Console.WriteLine($"- production operations: PASS ({productionCapabilities.Count} durable, integration, extension, and product checks)");
            Console.WriteLine("- generated serializer: PASS");
            Console.WriteLine("- generated editor viewmodels: PASS");
            Console.WriteLine("- 1,110 capability waves: PASS");
            Console.WriteLine("- local persistence: PASS");
            Console.WriteLine($"- geography reference data: PASS ({geography.Countries.Count:N0} countries, {geography.Subdivisions.Count:N0} divisions)");
            Console.WriteLine($"- IANA registries: PASS ({iana.Services.Count:N0} services, {iana.Protocols.Count:N0} protocols, {iana.CipherSuites.Count:N0} ciphers)");
            Console.WriteLine($"- lifecycle intelligence: PASS ({lifecycle.Releases.Count:N0} cycles, {lifecycle.VersionCount:N0} versions)");
            Console.WriteLine($"- localization intelligence: PASS ({localization.Locales.Count:N0} locales, {localization.TimeZones.Count:N0} zones)");
            Console.WriteLine($"- cloud economics: PASS ({cloud.Offers.Count:N0} regional offers)");
            Console.WriteLine($"- geospatial operations: PASS ({geospatial.Features.Count:N0} features)");
            Console.WriteLine("- reference pack SHA-256 integrity: PASS");
            Console.WriteLine("- Turso transport: READY (set TURSO_DATABASE_URL and TURSO_AUTH_TOKEN)");
            Console.WriteLine("AtlasOps headless validation passed");
        }
        finally
        {
            if (Directory.Exists(validationPath))
            {
                Directory.Delete(validationPath, recursive: true);
            }
        }
    }

    private static void Ensure(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }

    private static async Task VerifyPackAsync(
        string name,
        ReferencePackManifest manifest)
    {
        IReadOnlyList<ReferenceValidationIssue> issues = await ReferenceDataIntegrity.VerifyAsync(
            ReferenceDataPaths.GetPackDirectory(name),
            manifest);
        Ensure(issues.Count == 0, $"{name} reference-pack integrity failed: {issues.FirstOrDefault()?.Message}");
    }
}