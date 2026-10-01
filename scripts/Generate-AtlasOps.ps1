$ErrorActionPreference = 'Stop'

$projectRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$generatedRoot = Join-Path $projectRoot 'src\AtlasOps.Core\Models\Generated'

function Convert-ToPascalCase([string] $value) {
    return ($value -split '[^a-zA-Z0-9]' | Where-Object { $_ } | ForEach-Object {
        $_.Substring(0, 1).ToUpperInvariant() + $_.Substring(1)
    }) -join ''
}

function Get-DefaultExpression([string] $type, [string] $defaultValue) {
    if ($defaultValue) {
        return $defaultValue
    }

    switch ($type) {
        'string' { return 'string.Empty' }
        'bool' { return 'false' }
        'int' { return '0' }
        'long' { return '0L' }
        'double' { return '0D' }
        'decimal' { return '0M' }
        'DateTimeOffset' { return 'DateTimeOffset.UtcNow' }
        'Guid' { return 'Guid.NewGuid()' }
        default { return 'default!' }
    }
}

$models = @(
    @{ Name = 'Project'; Category = 'Workspace'; DisplayName = 'Projects'; Properties = @(
        @{ Name = 'Name'; Type = 'string' }, @{ Name = 'Stage'; Type = 'string'; Default = '"Discovery"' },
        @{ Name = 'Owner'; Type = 'string' }, @{ Name = 'Priority'; Type = 'int'; Default = '3' },
        @{ Name = 'IsPinned'; Type = 'bool' }, @{ Name = 'Notes'; Type = 'string' }
    ) },
    @{ Name = 'Connection'; Category = 'Connections'; DisplayName = 'Connections'; Properties = @(
        @{ Name = 'Name'; Type = 'string' }, @{ Name = 'Host'; Type = 'string' },
        @{ Name = 'Database'; Type = 'string' }, @{ Name = 'Protocol'; Type = 'string'; Default = '"turso"' },
        @{ Name = 'IsEncrypted'; Type = 'bool'; Default = 'true' }, @{ Name = 'LastStatus'; Type = 'string'; Default = '"Ready"' }
    ) },
    @{ Name = 'Credential'; Category = 'Connections'; DisplayName = 'Credentials'; Properties = @(
        @{ Name = 'Name'; Type = 'string' }, @{ Name = 'UserName'; Type = 'string' },
        @{ Name = 'Provider'; Type = 'string'; Default = '"Environment"' }, @{ Name = 'SecretReference'; Type = 'string' },
        @{ Name = 'ExpiresAt'; Type = 'DateTimeOffset' }, @{ Name = 'IsManaged'; Type = 'bool' }
    ) },
    @{ Name = 'Environment'; Category = 'Workspace'; DisplayName = 'Environments'; Properties = @(
        @{ Name = 'Name'; Type = 'string' }, @{ Name = 'Region'; Type = 'string' },
        @{ Name = 'Tier'; Type = 'string'; Default = '"Development"' }, @{ Name = 'Health'; Type = 'string'; Default = '"Healthy"' },
        @{ Name = 'Endpoint'; Type = 'string' }, @{ Name = 'IsProduction'; Type = 'bool' }
    ) },
    @{ Name = 'Host'; Category = 'Infrastructure'; DisplayName = 'Hosts'; Properties = @(
        @{ Name = 'Name'; Type = 'string' }, @{ Name = 'Address'; Type = 'string' },
        @{ Name = 'OperatingSystem'; Type = 'string' }, @{ Name = 'Architecture'; Type = 'string'; Default = '"x64"' },
        @{ Name = 'Status'; Type = 'string'; Default = '"Online"' }, @{ Name = 'CpuLoad'; Type = 'double' }
    ) },
    @{ Name = 'Service'; Category = 'Infrastructure'; DisplayName = 'Services'; Properties = @(
        @{ Name = 'Name'; Type = 'string' }, @{ Name = 'Kind'; Type = 'string' },
        @{ Name = 'Version'; Type = 'string' }, @{ Name = 'Status'; Type = 'string'; Default = '"Running"' },
        @{ Name = 'Replicas'; Type = 'int'; Default = '1' }, @{ Name = 'Endpoint'; Type = 'string' }
    ) },
    @{ Name = 'Deployment'; Category = 'Operations'; DisplayName = 'Deployments'; Properties = @(
        @{ Name = 'Name'; Type = 'string' }, @{ Name = 'EnvironmentName'; Type = 'string' },
        @{ Name = 'Version'; Type = 'string' }, @{ Name = 'Status'; Type = 'string'; Default = '"Queued"' },
        @{ Name = 'RequestedBy'; Type = 'string' }, @{ Name = 'Progress'; Type = 'int' }
    ) },
    @{ Name = 'Runbook'; Category = 'Operations'; DisplayName = 'Runbooks'; Properties = @(
        @{ Name = 'Title'; Type = 'string' }, @{ Name = 'Category'; Type = 'string' },
        @{ Name = 'Content'; Type = 'string' }, @{ Name = 'LastExecutedBy'; Type = 'string' },
        @{ Name = 'ExecutionCount'; Type = 'int' }, @{ Name = 'IsApproved'; Type = 'bool' }
    ) },
    @{ Name = 'Incident'; Category = 'Operations'; DisplayName = 'Incidents'; Properties = @(
        @{ Name = 'Title'; Type = 'string' }, @{ Name = 'Severity'; Type = 'string'; Default = '"Medium"' },
        @{ Name = 'Status'; Type = 'string'; Default = '"Open"' }, @{ Name = 'Commander'; Type = 'string' },
        @{ Name = 'Summary'; Type = 'string' }, @{ Name = 'AffectedServices'; Type = 'int' }
    ) },
    @{ Name = 'Alert'; Category = 'Operations'; DisplayName = 'Alerts'; Properties = @(
        @{ Name = 'Title'; Type = 'string' }, @{ Name = 'Source'; Type = 'string' },
        @{ Name = 'Severity'; Type = 'string'; Default = '"Warning"' }, @{ Name = 'Status'; Type = 'string'; Default = '"Active"' },
        @{ Name = 'Message'; Type = 'string' }, @{ Name = 'IsAcknowledged'; Type = 'bool' }
    ) },
    @{ Name = 'TaskItem'; Category = 'Planning'; DisplayName = 'Tasks'; Properties = @(
        @{ Name = 'Title'; Type = 'string' }, @{ Name = 'Status'; Type = 'string'; Default = '"To Do"' },
        @{ Name = 'Assignee'; Type = 'string'; Default = '"Unassigned"' }, @{ Name = 'Completion'; Type = 'int' },
        @{ Name = 'IsHighPriority'; Type = 'bool' }, @{ Name = 'DueAt'; Type = 'DateTimeOffset'; Default = 'DateTimeOffset.UtcNow.AddDays(3)' }
    ) },
    @{ Name = 'Milestone'; Category = 'Planning'; DisplayName = 'Milestones'; Properties = @(
        @{ Name = 'Name'; Type = 'string' }, @{ Name = 'Status'; Type = 'string'; Default = '"Planned"' },
        @{ Name = 'Owner'; Type = 'string' }, @{ Name = 'TargetDate'; Type = 'DateTimeOffset'; Default = 'DateTimeOffset.UtcNow.AddDays(14)' },
        @{ Name = 'Completion'; Type = 'int' }, @{ Name = 'IsBlocked'; Type = 'bool' }
    ) },
    @{ Name = 'EditorDocument'; Category = 'Editor'; DisplayName = 'Documents'; Properties = @(
        @{ Name = 'Title'; Type = 'string'; Default = '"Untitled"' }, @{ Name = 'Content'; Type = 'string' },
        @{ Name = 'Language'; Type = 'string'; Default = '"markdown"' }, @{ Name = 'FilePath'; Type = 'string' },
        @{ Name = 'IsDirty'; Type = 'bool' }, @{ Name = 'LastSavedAt'; Type = 'DateTimeOffset' }
    ) },
    @{ Name = 'Query'; Category = 'Editor'; DisplayName = 'Queries'; Properties = @(
        @{ Name = 'Name'; Type = 'string' }, @{ Name = 'Sql'; Type = 'string' },
        @{ Name = 'ConnectionName'; Type = 'string' }, @{ Name = 'LastDurationMs'; Type = 'long' },
        @{ Name = 'LastRowCount'; Type = 'int' }, @{ Name = 'IsFavorite'; Type = 'bool' }
    ) },
    @{ Name = 'Dashboard'; Category = 'Observability'; DisplayName = 'Dashboards'; Properties = @(
        @{ Name = 'Name'; Type = 'string' }, @{ Name = 'Layout'; Type = 'string'; Default = '"Grid"' },
        @{ Name = 'Owner'; Type = 'string' }, @{ Name = 'RefreshSeconds'; Type = 'int'; Default = '30' },
        @{ Name = 'WidgetCount'; Type = 'int' }, @{ Name = 'IsShared'; Type = 'bool' }
    ) },
    @{ Name = 'DashboardCard'; Category = 'Observability'; DisplayName = 'Dashboard cards'; Properties = @(
        @{ Name = 'Title'; Type = 'string' }, @{ Name = 'Value'; Type = 'string' },
        @{ Name = 'Trend'; Type = 'string' }, @{ Name = 'Kind'; Type = 'string'; Default = '"Info"' },
        @{ Name = 'OrderIndex'; Type = 'int' }, @{ Name = 'QueryName'; Type = 'string' }
    ) },
    @{ Name = 'AuditEvent'; Category = 'Governance'; DisplayName = 'Audit events'; Properties = @(
        @{ Name = 'Action'; Type = 'string' }, @{ Name = 'Actor'; Type = 'string' },
        @{ Name = 'Target'; Type = 'string' }, @{ Name = 'Result'; Type = 'string'; Default = '"Success"' },
        @{ Name = 'Details'; Type = 'string' }, @{ Name = 'OccurredAt'; Type = 'DateTimeOffset' }
    ) },
    @{ Name = 'Policy'; Category = 'Governance'; DisplayName = 'Policies'; Properties = @(
        @{ Name = 'Name'; Type = 'string' }, @{ Name = 'Scope'; Type = 'string' },
        @{ Name = 'Rule'; Type = 'string' }, @{ Name = 'Enforcement'; Type = 'string'; Default = '"Audit"' },
        @{ Name = 'Priority'; Type = 'int' }, @{ Name = 'IsEnabled'; Type = 'bool'; Default = 'true' }
    ) },
    @{ Name = 'Team'; Category = 'Organization'; DisplayName = 'Teams'; Properties = @(
        @{ Name = 'Name'; Type = 'string' }, @{ Name = 'Lead'; Type = 'string' },
        @{ Name = 'Description'; Type = 'string' }, @{ Name = 'MemberCount'; Type = 'int' },
        @{ Name = 'OnCallAlias'; Type = 'string' }, @{ Name = 'IsActive'; Type = 'bool'; Default = 'true' }
    ) },
    @{ Name = 'Activity'; Category = 'Organization'; DisplayName = 'Activities'; Properties = @(
        @{ Name = 'Title'; Type = 'string' }, @{ Name = 'Kind'; Type = 'string' },
        @{ Name = 'Actor'; Type = 'string' }, @{ Name = 'Description'; Type = 'string' },
        @{ Name = 'OccurredAt'; Type = 'DateTimeOffset' }, @{ Name = 'IsUnread'; Type = 'bool'; Default = 'true' }
    ) }
)

if (Test-Path $generatedRoot) {
    Get-ChildItem -Path $generatedRoot -Filter '*.g.cs' | Remove-Item -Force
}
else {
    New-Item -ItemType Directory -Path $generatedRoot -Force | Out-Null
}

foreach ($model in $models) {
    $className = 'AtlasOps' + (Convert-ToPascalCase $model.Name)
    $properties = @(
        '    public string Id { get; set; } = Guid.NewGuid().ToString("N");'
        '    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;'
    )

    foreach ($property in $model.Properties) {
        $defaultExpression = Get-DefaultExpression $property.Type $property.Default
        $properties += "    public $($property.Type) $($property.Name) { get; set; } = $defaultExpression;"
    }

    $source = @"
// <auto-generated />
namespace AtlasOps.Core;

[GenerateAtlasOpsModel("$($model.Category)", "$($model.DisplayName)")]
public sealed partial class $className : IAtlasOpsEntity
{
$($properties -join [Environment]::NewLine)
}
"@

    $path = Join-Path $generatedRoot "$className.g.cs"
    [System.IO.File]::WriteAllText($path, $source, [System.Text.UTF8Encoding]::new($false))
}

$manifestPath = Join-Path $generatedRoot 'AtlasOpsModelManifest.g.cs'
$manifestEntries = $models | ForEach-Object {
    $className = 'AtlasOps' + (Convert-ToPascalCase $_.Name)
    "        typeof($className),"
}
$manifestSource = @"
// <auto-generated />
namespace AtlasOps.Core;

public static class AtlasOpsModelManifest
{
    public static IReadOnlyList<Type> Types { get; } =
    [
$($manifestEntries -join [Environment]::NewLine)
    ];
}
"@
[System.IO.File]::WriteAllText($manifestPath, $manifestSource, [System.Text.UTF8Encoding]::new($false))

Write-Host "Generated $($models.Count) AtlasOps models and the model manifest in $generatedRoot"
