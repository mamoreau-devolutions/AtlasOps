$ErrorActionPreference = 'Stop'

$projectRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$featureRoot = Join-Path $projectRoot 'src\AtlasOps.Features\Generated'
$testRoot = Join-Path $projectRoot 'tests\AtlasOps.Tests\GeneratedWaves'

$waves = @(
    @{ Number = 1; Area = 'Platform'; Capabilities = @('ModuleRegistration', 'NavigationRouting', 'CommandDispatch', 'BackgroundOperation', 'NotificationDelivery', 'DialogCoordination', 'SettingsManagement', 'LayoutPersistence', 'FeatureDiscovery', 'WorkspaceLifecycle') },
    @{ Number = 2; Area = 'Connections'; Capabilities = @('SshConnection', 'RdpConnection', 'HttpEndpoint', 'DatabaseConnection', 'KubernetesContext', 'LocalProcess', 'CredentialReference', 'SecretLease', 'ConnectionTemplate', 'ConnectionImport') },
    @{ Number = 3; Area = 'Inventory'; Capabilities = @('EnvironmentInventory', 'SiteInventory', 'NetworkInventory', 'HostInventory', 'ServiceInventory', 'DependencyMapping', 'HealthObservation', 'DiscoveryScan', 'ReconciliationPlan', 'TopologyProjection') },
    @{ Number = 4; Area = 'Automation'; Capabilities = @('DeploymentPlan', 'DeploymentStage', 'RunbookDefinition', 'RunbookExecution', 'ApprovalRequest', 'ChangeWindow', 'RollbackPlan', 'ExecutionCheckpoint', 'StepRetry', 'WorkflowSimulation') },
    @{ Number = 5; Area = 'Incidents'; Capabilities = @('AlertRule', 'AlertCorrelation', 'AlertSuppression', 'IncidentCase', 'IncidentTimeline', 'ResponderAssignment', 'EscalationPolicy', 'IncidentNotification', 'PostmortemReview', 'IncidentActionItem') },
    @{ Number = 6; Area = 'Editor'; Capabilities = @('DocumentSession', 'AutosaveJournal', 'QueryDefinition', 'QueryExecution', 'QueryParameter', 'ResultProjection', 'SyntaxProfile', 'CommandHistory', 'TerminalSession', 'WorkspaceRecovery') },
    @{ Number = 7; Area = 'Analytics'; Capabilities = @('MetricDefinition', 'MetricSample', 'MetricRetention', 'DashboardLayout', 'NumberWidget', 'TrendWidget', 'TableWidget', 'HealthWidget', 'ReportDefinition', 'ReportSchedule') },
    @{ Number = 8; Area = 'Governance'; Capabilities = @('ActorIdentity', 'TeamMembership', 'RoleDefinition', 'PermissionGrant', 'ResourceScope', 'AuthorizationDecision', 'PolicyDefinition', 'PolicySimulation', 'AuditEnvelope', 'RetentionPolicy') },
    @{ Number = 9; Area = 'Sync'; Capabilities = @('SchemaMigration', 'DurableOutbox', 'DurableInbox', 'SyncCheckpoint', 'SyncBatch', 'ConflictDetection', 'ConflictResolution', 'BackupArchive', 'RestoreOperation', 'ImportExport') },
    @{ Number = 10; Area = 'Hardening'; Capabilities = @('DiagnosticSnapshot', 'SupportBundle', 'CrashMarker', 'AccessibilityAudit', 'PerformanceBudget', 'StartupProbe', 'RecoveryMode', 'PackageManifest', 'UpgradeAssessment', 'ReleaseReadiness') }
)

$expansionPrograms = @(
    @{ Area = 'Platform'; Capabilities = @('ExtensionMarketplace', 'ModuleIsolation', 'FeatureToggle', 'CommandTelemetry', 'OperationThrottling', 'CacheCoordination', 'ClockAbstraction', 'LocalizationCatalog', 'ThemeComposition', 'SessionLifecycle') },
    @{ Area = 'Connections'; Capabilities = @('ProxyProfile', 'TunnelDefinition', 'PortForwarding', 'CertificateTrust', 'HostKeyVerification', 'ConnectionPool', 'SessionRecording', 'FileTransfer', 'RemoteClipboard', 'GatewayRouting') },
    @{ Area = 'Inventory'; Capabilities = @('CloudAccount', 'ClusterInventory', 'ContainerInventory', 'DatabaseInventory', 'CertificateInventory', 'LicenseInventory', 'CostObservation', 'CapacityForecast', 'DriftDetection', 'AssetLifecycle') },
    @{ Area = 'Automation'; Capabilities = @('WorkflowVariable', 'ConditionalStep', 'ParallelStage', 'ScheduledRunbook', 'ManualGate', 'ArtifactPromotion', 'DeploymentFreeze', 'CanaryRollout', 'BlueGreenRollout', 'AutomationCredential') },
    @{ Area = 'Incidents'; Capabilities = @('ServiceLevelObjective', 'ErrorBudget', 'OnCallSchedule', 'RotationHandoff', 'AlertEnrichment', 'IncidentTemplate', 'StatusUpdate', 'StakeholderSubscription', 'CommunicationChannel', 'RemediationTracking') },
    @{ Area = 'Editor'; Capabilities = @('NotebookDocument', 'ScriptLibrary', 'SnippetCatalog', 'SchemaBrowser', 'QueryPlan', 'ResultComparison', 'DataExport', 'TerminalProfile', 'SessionTranscript', 'WorkspaceBookmark') },
    @{ Area = 'Analytics'; Capabilities = @('GaugeWidget', 'TimelineWidget', 'TopologyWidget', 'MarkdownWidget', 'QueryWidget', 'DashboardParameter', 'SharedFilter', 'MetricFormula', 'ReportTemplate', 'AnalyticsSubscription') },
    @{ Area = 'Governance'; Capabilities = @('DataClassification', 'ComplianceControl', 'PolicyException', 'ReviewCampaign', 'AccessRequest', 'OwnershipRule', 'SegregationOfDuties', 'AuditExport', 'LegalHold', 'GovernanceAttestation') },
    @{ Area = 'Sync'; Capabilities = @('ReplicationPeer', 'ChangeVector', 'TombstoneRecord', 'MergeStrategy', 'OfflineQueue', 'BandwidthPolicy', 'SyncSchedule', 'DataArchive', 'WorkspaceClone', 'DisasterRecovery') },
    @{ Area = 'Hardening'; Capabilities = @('FeatureHealth', 'DependencyProbe', 'MemoryBudget', 'ResponsivenessProbe', 'AccessibilityProfile', 'KeyboardMap', 'LocalizationAudit', 'UpgradeChannel', 'RollbackPackage', 'ReleaseEvidence') }
)

$nextWave = 11
foreach ($program in $expansionPrograms) {
    foreach ($capability in $program.Capabilities) {
        $waves += @{
            Number = $nextWave
            Area = $program.Area
            Capabilities = @($capability)
        }
        $nextWave++
    }
}

$scalePrograms = @(
    @{ Area = 'Cloud'; Resources = @('AwsAccount', 'AzureSubscription', 'GcpProject', 'CloudRegion', 'CloudNetwork', 'CloudIdentity', 'CloudDatabase', 'CloudStorage', 'CloudFunction', 'CloudBilling') },
    @{ Area = 'Kubernetes'; Resources = @('KubernetesCluster', 'KubernetesNamespace', 'KubernetesWorkload', 'KubernetesPod', 'KubernetesService', 'KubernetesIngress', 'KubernetesConfig', 'KubernetesSecret', 'KubernetesVolume', 'KubernetesOperator') },
    @{ Area = 'Network'; Resources = @('NetworkSegment', 'NetworkRoute', 'NetworkFirewall', 'NetworkLoadBalancer', 'NetworkDnsZone', 'NetworkVpn', 'NetworkPeer', 'NetworkAddress', 'NetworkProbe', 'NetworkPolicy') },
    @{ Area = 'Security'; Resources = @('SecurityFinding', 'SecurityScan', 'SecurityBaseline', 'SecurityPatch', 'SecurityCertificate', 'SecurityKey', 'SecurityIdentity', 'SecuritySession', 'SecurityBoundary', 'SecurityException') },
    @{ Area = 'Database'; Resources = @('SqlDatabase', 'NoSqlDatabase', 'DatabaseSchema', 'DatabaseReplica', 'DatabaseBackup', 'DatabaseRestore', 'DatabaseIndex', 'DatabaseQuery', 'DatabaseCredential', 'DatabaseMaintenance') },
    @{ Area = 'Storage'; Resources = @('ObjectBucket', 'FileShare', 'BlockVolume', 'StorageSnapshot', 'StorageArchive', 'StorageReplication', 'StorageQuota', 'StorageEncryption', 'StorageLifecycle', 'StorageTransfer') },
    @{ Area = 'Messaging'; Resources = @('MessageBroker', 'MessageTopic', 'MessageQueue', 'MessageSubscription', 'MessageSchema', 'MessageConsumer', 'MessageProducer', 'MessageDeadLetter', 'MessageReplay', 'MessageRetention') },
    @{ Area = 'Api'; Resources = @('ApiGateway', 'ApiEndpoint', 'ApiContract', 'ApiClient', 'ApiToken', 'ApiQuota', 'ApiVersion', 'ApiDeployment', 'ApiHealth', 'ApiAnalytics') },
    @{ Area = 'Compute'; Resources = @('VirtualMachine', 'ComputeImage', 'ComputeTemplate', 'ComputeScaleSet', 'ComputeReservation', 'ComputeSchedule', 'ComputePatch', 'ComputeConsole', 'ComputeMetric', 'ComputeLifecycle') },
    @{ Area = 'Delivery'; Resources = @('SourceRepository', 'BuildPipeline', 'BuildArtifact', 'ReleasePipeline', 'ReleaseEnvironment', 'ReleaseGate', 'ReleaseApproval', 'ReleaseRollback', 'ReleaseMetric', 'ReleaseCalendar') },
    @{ Area = 'Observability'; Resources = @('LogSource', 'LogQuery', 'TraceSource', 'TraceSpan', 'MetricSource', 'MetricAlert', 'ObservabilityDashboard', 'ObservabilitySlo', 'ObservabilityExport', 'ObservabilityRetention') },
    @{ Area = 'FinOps'; Resources = @('CostCenter', 'CloudInvoice', 'BudgetPlan', 'SpendForecast', 'CostAllocation', 'SavingsPlan', 'ResourceCommitment', 'ChargebackRule', 'CostAnomaly', 'FinOpsReport') },
    @{ Area = 'ServiceManagement'; Resources = @('ServiceCatalog', 'ServiceOwner', 'ServiceDependency', 'ServiceRequest', 'ChangeRequest', 'ProblemRecord', 'KnowledgeArticle', 'MaintenanceWindow', 'ServiceReview', 'ServiceScorecard') },
    @{ Area = 'Data'; Resources = @('DataSource', 'DataPipeline', 'DataDataset', 'DataContract', 'DataQuality', 'DataLineage', 'DataTransform', 'DataRetention', 'DataAccess', 'DataProduct') },
    @{ Area = 'Identity'; Resources = @('IdentityProvider', 'IdentityUser', 'IdentityGroup', 'IdentityRole', 'IdentitySession', 'IdentityFactor', 'IdentityApplication', 'IdentityClaim', 'IdentityLifecycle', 'IdentityAudit') },
    @{ Area = 'Edge'; Resources = @('EdgeSite', 'EdgeDevice', 'EdgeGateway', 'EdgeApplication', 'EdgeDeployment', 'EdgeNetwork', 'EdgeTelemetry', 'EdgePolicy', 'EdgeUpdate', 'EdgeIncident') },
    @{ Area = 'Desktop'; Resources = @('DesktopPool', 'DesktopImage', 'DesktopSession', 'DesktopApplication', 'DesktopPolicy', 'DesktopProfile', 'DesktopUpdate', 'DesktopPeripheral', 'DesktopHealth', 'DesktopLicense') },
    @{ Area = 'Mobile'; Resources = @('MobileFleet', 'MobileDevice', 'MobileApplication', 'MobileProfile', 'MobilePolicy', 'MobileCertificate', 'MobileUpdate', 'MobileCompliance', 'MobileTelemetry', 'MobileSupport') },
    @{ Area = 'BusinessContinuity'; Resources = @('ContinuityPlan', 'RecoverySite', 'RecoveryRunbook', 'RecoveryExercise', 'RecoveryObjective', 'RecoveryDependency', 'RecoveryBackup', 'RecoveryFailover', 'RecoveryEvidence', 'RecoveryReview') },
    @{ Area = 'Architecture'; Resources = @('ArchitectureDecision', 'ArchitectureComponent', 'ArchitectureInterface', 'ArchitectureDependency', 'ArchitectureStandard', 'ArchitectureException', 'ArchitectureRoadmap', 'ArchitectureRisk', 'ArchitectureReview', 'ArchitectureEvidence') }
)

$scaleConcerns = @('Provisioning', 'Monitoring', 'Governance', 'Optimization', 'Recovery')
foreach ($program in $scalePrograms) {
    foreach ($resource in $program.Resources) {
        foreach ($concern in $scaleConcerns) {
            $waves += @{
                Number = $nextWave
                Area = $program.Area
                Capabilities = @("$resource$concern")
            }
            $nextWave++
        }
    }
}

function Write-GeneratedFile([string] $path, [string] $content) {
    $directory = Split-Path $path -Parent
    New-Item -ItemType Directory -Path $directory -Force | Out-Null
    [System.IO.File]::WriteAllText($path, $content, [System.Text.UTF8Encoding]::new($false))
}

if (Test-Path $featureRoot) {
    Remove-Item $featureRoot -Recurse -Force
}

if (Test-Path $testRoot) {
    Remove-Item $testRoot -Recurse -Force
}

$contracts = @'
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
'@
Write-GeneratedFile (Join-Path $featureRoot 'AtlasOpsCapabilityContracts.g.cs') $contracts

$catalogEntries = [System.Collections.Generic.List[string]]::new()
$manifestEntries = [System.Collections.Generic.List[string]]::new()
$generatedCount = 1

foreach ($wave in $waves) {
    $area = $wave.Area
    $waveNumber = $wave.Number
    $priority = (($waveNumber - 1) % 10) + 1

    foreach ($capability in $wave.Capabilities) {
        $displayName = [regex]::Replace($capability, '([a-z0-9])([A-Z])', '$1 $2')
        $namespace = "AtlasOps.Features.$area.$capability"
        $directory = Join-Path $featureRoot "Wave$waveNumber\$area\$capability"
        $modelName = "$capability" + 'Item'
        $eventName = "$capability" + 'Changed'
        $commandName = "Update$capability" + 'Command'
        $validatorName = "$capability" + 'Validator'
        $policyName = "$capability" + 'Policy'
        $repositoryName = "I$capability" + 'Repository'
        $serviceName = "$capability" + 'Service'
        $viewModelName = "$capability" + 'ViewModel'
        $viewName = "$capability" + 'View'
        $capabilityId = "$($area.ToLowerInvariant()).$($capability.ToLowerInvariant())"

        $model = @"
namespace $namespace;

using AtlasOps.Features;

public sealed class $modelName : IAtlasOpsCapabilityEntity
{
    public string Id { get; init; } = Guid.NewGuid().ToString("N");

    public string Name { get; set; } = "$displayName";

    public string Owner { get; set; } = "Operations";

    public string State { get; set; } = "Draft";

    public int Priority { get; set; } = $priority;

    public bool IsEnabled { get; set; } = true;

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public int Revision { get; private set; }

    public void MarkUpdated(DateTimeOffset timestamp)
    {
        this.UpdatedAt = timestamp;
        this.Revision++;
    }
}
"@
        Write-GeneratedFile (Join-Path $directory "$modelName.g.cs") $model

        $command = @"
namespace $namespace;

public sealed record $commandName(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);
"@
        Write-GeneratedFile (Join-Path $directory "$commandName.g.cs") $command

        $validator = @"
namespace $namespace;

using AtlasOps.Features;

public sealed class $validatorName
{
    public IReadOnlyList<AtlasOpsValidationIssue> Validate($commandName command)
    {
        List<AtlasOpsValidationIssue> issues = [];

        if (string.IsNullOrWhiteSpace(command.Id))
        {
            issues.Add(new("Id", "An identifier is required."));
        }

        if (string.IsNullOrWhiteSpace(command.Name))
        {
            issues.Add(new("Name", "A name is required."));
        }

        if (string.IsNullOrWhiteSpace(command.Owner))
        {
            issues.Add(new("Owner", "An owner is required."));
        }

        if (string.IsNullOrWhiteSpace(command.TargetState))
        {
            issues.Add(new("State", "A target state is required."));
        }

        if (command.Priority is < 1 or > 10)
        {
            issues.Add(new("Priority", "Priority must be between 1 and 10."));
        }

        return issues;
    }
}
"@
        Write-GeneratedFile (Join-Path $directory "$validatorName.g.cs") $validator

        $policy = @"
namespace $namespace;

public sealed class $policyName
{
    private static readonly IReadOnlyDictionary<string, IReadOnlySet<string>> Transitions =
        new Dictionary<string, IReadOnlySet<string>>(StringComparer.OrdinalIgnoreCase)
        {
            ["Draft"] = new HashSet<string>(["Ready", "Cancelled"], StringComparer.OrdinalIgnoreCase),
            ["Ready"] = new HashSet<string>(["Running", "Cancelled"], StringComparer.OrdinalIgnoreCase),
            ["Running"] = new HashSet<string>(["Completed", "Failed", "Paused"], StringComparer.OrdinalIgnoreCase),
            ["Paused"] = new HashSet<string>(["Running", "Cancelled"], StringComparer.OrdinalIgnoreCase),
            ["Failed"] = new HashSet<string>(["Ready", "Cancelled"], StringComparer.OrdinalIgnoreCase),
            ["Completed"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase),
            ["Cancelled"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase),
        };

    public bool CanTransition(string currentState, string targetState)
    {
        return Transitions.TryGetValue(currentState, out IReadOnlySet<string>? targets) && targets.Contains(targetState);
    }

    public IReadOnlyList<string> GetAvailableTransitions(string currentState)
    {
        return Transitions.TryGetValue(currentState, out IReadOnlySet<string>? targets)
            ? targets.Order(StringComparer.OrdinalIgnoreCase).ToArray()
            : [];
    }
}
"@
        Write-GeneratedFile (Join-Path $directory "$policyName.g.cs") $policy

        $repository = @"
namespace $namespace;

using AtlasOps.Features;

public interface $repositoryName : IAtlasOpsCapabilityRepository<$modelName>;
"@
        Write-GeneratedFile (Join-Path $directory "$repositoryName.g.cs") $repository

        $eventSource = @"
namespace $namespace;

public sealed record $eventName(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);
"@
        Write-GeneratedFile (Join-Path $directory "$eventName.g.cs") $eventSource

        $service = @"
namespace $namespace;

using AtlasOps.Features;

public sealed class $serviceName(
    IAtlasOpsCapabilityRepository<$modelName> repository,
    TimeProvider timeProvider)
{
    private readonly $validatorName validator = new();
    private readonly $policyName policy = new();

    public async Task<AtlasOpsOperationResult<$eventName>> ExecuteAsync(
        $commandName command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<$eventName>.Invalid(issues);
        }

        $modelName entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new $modelName { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<$eventName>.Invalid(
            [
                new("State", `$"Cannot transition from '{previousState}' to '{command.TargetState}'."),
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

        $eventName changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<$eventName>.Success(changed);
    }
}
"@
        Write-GeneratedFile (Join-Path $directory "$serviceName.g.cs") $service

        $viewModel = @"
namespace $namespace;

using System.ComponentModel;
using System.Runtime.CompilerServices;

public sealed class $viewModelName : INotifyPropertyChanged
{
    private readonly $policyName policy = new();
    private string state = "Draft";
    private string status = "Ready for $displayName operations.";

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Title => "$displayName";

    public string Area => "$area";

    public int Wave => $waveNumber;

    public string State
    {
        get => this.state;
        private set
        {
            if (string.Equals(this.state, value, StringComparison.Ordinal))
            {
                return;
            }

            this.state = value;
            this.OnPropertyChanged();
            this.OnPropertyChanged(nameof(this.AvailableActions));
        }
    }

    public string Status
    {
        get => this.status;
        private set
        {
            this.status = value;
            this.OnPropertyChanged();
        }
    }

    public IReadOnlyList<string> AvailableActions => this.policy.GetAvailableTransitions(this.State);

    public void Advance()
    {
        string? next = this.AvailableActions.FirstOrDefault();
        if (next is null)
        {
            this.Status = `$"{this.Title} is in terminal state {this.State}.";
            return;
        }

        this.State = next;
        this.Status = `$"{this.Title} moved to {this.State}.";
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new(propertyName));
    }
}
"@
        Write-GeneratedFile (Join-Path $directory "$viewModelName.g.cs") $viewModel

        $view = @"
<local:$viewName
    xmlns="https://github.com/avaloniaui"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:local="clr-namespace:$namespace"
    x:Class="$namespace.$viewName"
    x:DataType="local:$viewModelName"
    MinWidth="320"
    MinHeight="180">
    <Border Padding="16" CornerRadius="8" Background="#17243A">
        <Grid RowDefinitions="Auto,Auto,Auto,*" ColumnDefinitions="*,Auto">
            <TextBlock Text="{Binding Title}" FontSize="18" FontWeight="SemiBold" />
            <Border Grid.Column="1" Padding="8,4" CornerRadius="10" Background="#294263">
                <TextBlock Text="{Binding State}" />
            </Border>
            <TextBlock Grid.Row="1" Grid.ColumnSpan="2" Margin="0,6,0,0" Text="{Binding Area, StringFormat='Wave $waveNumber · {0}'}" Foreground="#91A1B8" />
            <TextBlock Grid.Row="2" Grid.ColumnSpan="2" Margin="0,12,0,0" Text="{Binding Status}" TextWrapping="Wrap" />
            <StackPanel Grid.Row="3" Grid.ColumnSpan="2" Margin="0,16,0,0" Orientation="Horizontal" Spacing="8">
                <Button Content="Advance" Click="Advance_OnClick" />
                <ItemsControl ItemsSource="{Binding AvailableActions}">
                    <ItemsControl.ItemsPanel>
                        <ItemsPanelTemplate>
                            <StackPanel Orientation="Horizontal" Spacing="6" />
                        </ItemsPanelTemplate>
                    </ItemsControl.ItemsPanel>
                    <ItemsControl.ItemTemplate>
                        <DataTemplate>
                            <Border Padding="6,3" CornerRadius="4" Background="#203653">
                                <TextBlock Text="{Binding}" FontSize="11" />
                            </Border>
                        </DataTemplate>
                    </ItemsControl.ItemTemplate>
                </ItemsControl>
            </StackPanel>
        </Grid>
    </Border>
</local:$viewName>
"@
        Write-GeneratedFile (Join-Path $directory "$viewName.axaml") $view

        $codeBehind = @"
namespace $namespace;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class $viewName : UserControl
{
    public $viewName()
    {
        this.DataContext = new $viewModelName();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is $viewModelName viewModel)
        {
            viewModel.Advance();
        }
    }
}
"@
        Write-GeneratedFile (Join-Path $directory "$viewName.axaml.cs") $codeBehind

        $test = @"
namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using $namespace;

[TestClass]
public sealed class $($capability)Tests
{
    private static $commandName CreateCommand(string targetState = "Ready")
    {
        return new("$capabilityId-1", "$displayName", "Operations", targetState, $priority, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "$capabilityId");

        Assert.AreEqual($waveNumber, descriptor.Wave);
        Assert.AreEqual("$area", descriptor.Area);
        Assert.AreEqual(typeof($modelName), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        $validatorName validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        $validatorName validator = new();
        $commandName command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        $policyName policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<$modelName> repository = new();
        $serviceName service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<$eventName> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        ${modelName}? stored = await repository.GetAsync("$capabilityId-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<$modelName> repository = new();
        $serviceName service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<$eventName> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        $viewModelName viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}
"@
        Write-GeneratedFile (Join-Path $testRoot "Wave$waveNumber\$($capability)Tests.g.cs") $test

        $catalogEntries.Add("        new(`"$capabilityId`", `"$displayName`", `"$area`", $waveNumber, typeof($namespace.$modelName), typeof($namespace.$viewModelName), typeof($namespace.$viewName), static () => new $namespace.$modelName(), static () => new $namespace.$viewModelName(), static () => new $namespace.$viewName()),")
        $manifestEntries.Add("        new($waveNumber, `"$area`", `"$displayName`", `"$capabilityId`"),")
        $generatedCount += 10
    }
}

$catalog = @"
namespace AtlasOps.Features;

public static class AtlasOpsCapabilityCatalog
{
    public static IReadOnlyList<AtlasOpsCapabilityDescriptor> All { get; } =
    [
$($catalogEntries -join [Environment]::NewLine)
    ];

    public static IReadOnlyList<AtlasOpsCapabilityDescriptor> GetWave(int wave)
    {
        return All.Where(item => item.Wave == wave).ToArray();
    }
}
"@
Write-GeneratedFile (Join-Path $featureRoot 'AtlasOpsCapabilityCatalog.g.cs') $catalog
$generatedCount++

$manifest = @"
namespace AtlasOps.Features;

public sealed record AtlasOpsWaveCapability(int Wave, string Area, string DisplayName, string Id);

public static class AtlasOpsWaveManifest
{
    public static IReadOnlyList<AtlasOpsWaveCapability> Capabilities { get; } =
    [
$($manifestEntries -join [Environment]::NewLine)
    ];

    public static IReadOnlyDictionary<int, int> CountsByWave { get; } =
        Capabilities.GroupBy(static item => item.Wave).ToDictionary(static group => group.Key, static group => group.Count());
}
"@
Write-GeneratedFile (Join-Path $featureRoot 'AtlasOpsWaveManifest.g.cs') $manifest
$generatedCount++

Write-Host "Generated $generatedCount feature source files and $($manifestEntries.Count) capability test files across $($waves.Count) waves."
