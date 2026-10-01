namespace AtlasOps.Enterprise.Core.Scenarios;

using AtlasOps.Enterprise.Contracts.Incidents;
using AtlasOps.Enterprise.Contracts.Inventory;
using AtlasOps.Enterprise.Contracts.Policy;
using AtlasOps.Enterprise.Contracts.Reporting;
using AtlasOps.Enterprise.Contracts.Workflow;

public static class EnterpriseScenarioCatalog
{
    public static WorkflowDefinition CreateDeploymentWorkflow()
    {
        WorkflowRetryPolicy standardRetry = new(3, TimeSpan.FromSeconds(10), 2d, TimeSpan.FromMinutes(2));
        WorkflowRetryPolicy noRetry = new(1, TimeSpan.Zero, 1d, TimeSpan.Zero);
        WorkflowStep validate = new(
            "validate",
            "Validate deployment",
            WorkflowStepKind.Action,
            "deployment.validate",
            new Dictionary<string, string> { ["environment"] = "production" },
            standardRetry);
        WorkflowStep approve = new(
            "approve",
            "Request approval",
            WorkflowStepKind.Approval,
            "approval.request",
            new Dictionary<string, string> { ["policy"] = "production-change" },
            noRetry);
        WorkflowStep deploy = new(
            "deploy",
            "Deploy release",
            WorkflowStepKind.Action,
            "deployment.execute",
            new Dictionary<string, string> { ["strategy"] = "canary" },
            standardRetry,
            "rollback",
            TimeSpan.FromMinutes(30));
        WorkflowStep verify = new(
            "verify",
            "Verify service health",
            WorkflowStepKind.Condition,
            "observability.verify",
            new Dictionary<string, string> { ["window"] = "10m" },
            standardRetry,
            "rollback");
        WorkflowStep notify = new(
            "notify",
            "Notify stakeholders",
            WorkflowStepKind.Notification,
            "notification.send",
            new Dictionary<string, string> { ["channel"] = "operations" },
            noRetry);
        WorkflowStep rollback = new(
            "rollback",
            "Roll back release",
            WorkflowStepKind.Action,
            "deployment.rollback",
            new Dictionary<string, string>(),
            standardRetry);
        return new(
            "production-deployment",
            "Production deployment",
            4,
            [validate, approve, deploy, verify, notify, rollback],
            [
                new("validate", "approve"),
                new("approve", "deploy"),
                new("deploy", "verify"),
                new("verify", "notify", "healthScore >= 95"),
            ],
            new Dictionary<string, string>
            {
                ["owner"] = "platform-engineering",
                ["changeClass"] = "standard",
            });
    }

    public static WorkflowDefinition CreateInvalidWorkflow()
    {
        WorkflowRetryPolicy retry = new(1, TimeSpan.Zero, 1d, TimeSpan.Zero);
        return new(
            "cyclic-recovery",
            "Cyclic recovery",
            1,
            [
                new("detect", "Detect failure", WorkflowStepKind.Action, "failure.detect", new Dictionary<string, string>(), retry),
                new("recover", "Recover service", WorkflowStepKind.Action, "service.recover", new Dictionary<string, string>(), retry),
            ],
            [
                new("detect", "recover"),
                new("recover", "detect"),
            ],
            new Dictionary<string, string>());
    }

    public static IReadOnlyList<RoleDefinition> CreateRoles()
    {
        return
        [
            new("viewer", "Viewer", new HashSet<string>(StringComparer.OrdinalIgnoreCase)),
            new("operator", "Operator", new HashSet<string>(["viewer"], StringComparer.OrdinalIgnoreCase)),
            new("incident-commander", "Incident commander", new HashSet<string>(["operator"], StringComparer.OrdinalIgnoreCase)),
            new("administrator", "Administrator", new HashSet<string>(["incident-commander"], StringComparer.OrdinalIgnoreCase)),
        ];
    }

    public static PolicySet CreateOperationsPolicy()
    {
        return new(
            "operations-access",
            "Operations access",
            [
                new(
                    "deny-production-delete",
                    "Protect production deletion",
                    PolicyEffect.Deny,
                    100,
                    new HashSet<string>(StringComparer.OrdinalIgnoreCase),
                    new HashSet<string>(["asset.delete"], StringComparer.OrdinalIgnoreCase),
                    new HashSet<string>(["asset"], StringComparer.OrdinalIgnoreCase),
                    [new("resource", "environment", PolicyConditionOperator.Equals, "production")]),
                new(
                    "allow-admin",
                    "Administrator access",
                    PolicyEffect.Allow,
                    90,
                    new HashSet<string>(["administrator"], StringComparer.OrdinalIgnoreCase),
                    new HashSet<string>(["*"], StringComparer.OrdinalIgnoreCase),
                    new HashSet<string>(["*"], StringComparer.OrdinalIgnoreCase),
                    []),
                new(
                    "allow-incident-update",
                    "Incident commanders update incidents",
                    PolicyEffect.Allow,
                    70,
                    new HashSet<string>(["incident-commander"], StringComparer.OrdinalIgnoreCase),
                    new HashSet<string>(["incident.read", "incident.update"], StringComparer.OrdinalIgnoreCase),
                    new HashSet<string>(["incident"], StringComparer.OrdinalIgnoreCase),
                    [new("environment", "network", PolicyConditionOperator.In, "corporate,vpn")]),
                new(
                    "allow-view",
                    "View operational resources",
                    PolicyEffect.Allow,
                    20,
                    new HashSet<string>(["viewer"], StringComparer.OrdinalIgnoreCase),
                    new HashSet<string>(["incident.read", "asset.read", "report.read"], StringComparer.OrdinalIgnoreCase),
                    new HashSet<string>(["incident", "asset", "report"], StringComparer.OrdinalIgnoreCase),
                    []),
            ]);
    }

    public static IReadOnlyList<PolicyRequest> CreatePolicyRequests()
    {
        return
        [
            new(
                new("alex", new HashSet<string>(["incident-commander"], StringComparer.OrdinalIgnoreCase), new Dictionary<string, string> { ["department"] = "operations" }),
                "incident.update",
                new("inc-1042", "incident", new Dictionary<string, string> { ["environment"] = "production" }),
                new Dictionary<string, string> { ["network"] = "vpn", ["risk"] = "medium" }),
            new(
                new("morgan", new HashSet<string>(["administrator"], StringComparer.OrdinalIgnoreCase), new Dictionary<string, string> { ["department"] = "platform" }),
                "asset.delete",
                new("server-021", "asset", new Dictionary<string, string> { ["environment"] = "production" }),
                new Dictionary<string, string> { ["network"] = "corporate", ["risk"] = "high" }),
            new(
                new("casey", new HashSet<string>(["viewer"], StringComparer.OrdinalIgnoreCase), new Dictionary<string, string> { ["department"] = "finance" }),
                "report.read",
                new("availability", "report", new Dictionary<string, string> { ["environment"] = "shared" }),
                new Dictionary<string, string> { ["network"] = "internet", ["risk"] = "low" }),
        ];
    }

    public static IReadOnlyList<Incident> CreateIncidents(DateTimeOffset now)
    {
        return
        [
            new("INC-1042", "Checkout latency above SLO", "P95 latency is above 2 seconds in two regions.", IncidentSeverity.Sev1, IncidentState.Investigating, "checkout-api", "sre-primary", now.AddMinutes(-48), now.AddMinutes(-4), now.AddMinutes(-43)),
            new("INC-1041", "Delayed inventory imports", "Cloud inventory ingestion is delayed.", IncidentSeverity.Sev2, IncidentState.Acknowledged, "inventory-ingestion", "platform-oncall", now.AddHours(-2), now.AddMinutes(-32), now.AddHours(-1.8)),
            new("INC-1038", "Dashboard refresh errors", "Some dashboards fail to refresh after schema changes.", IncidentSeverity.Sev3, IncidentState.Mitigated, "reporting", "analytics-team", now.AddHours(-8), now.AddMinutes(-52), now.AddHours(-7.5)),
            new("INC-1029", "Stale ownership labels", "Asset ownership labels are behind the source directory.", IncidentSeverity.Sev4, IncidentState.Resolved, "asset-catalog", "inventory-team", now.AddDays(-2), now.AddHours(-3), now.AddDays(-1.8), now.AddHours(-3)),
        ];
    }

    public static IReadOnlyList<ServiceDependency> CreateServiceDependencies()
    {
        return
        [
            new("web-store", "checkout-api", true, "synchronous request"),
            new("mobile-store", "checkout-api", true, "synchronous request"),
            new("checkout-api", "payment-gateway", true, "payment authorization"),
            new("checkout-api", "inventory-api", false, "availability lookup"),
            new("operations-dashboard", "reporting", false, "analytics feed"),
            new("reporting", "asset-catalog", false, "asset dimensions"),
        ];
    }

    public static IReadOnlyList<AssetRecord> CreateAssets(DateTimeOffset now)
    {
        return
        [
            new(
                "asset-web-01",
                "web-prod-01",
                "virtual-machine",
                "platform",
                AssetLifecycleState.Active,
                [new("cloud-resource-id", "/subscriptions/prod/vm/web-prod-01", true), new("hostname", "web-prod-01", false)],
                new Dictionary<string, string>
                {
                    ["environment"] = "production",
                    ["location"] = "canada-central",
                    ["operatingSystem"] = "Ubuntu 24.04",
                    ["encryption"] = "enabled",
                    ["owner"] = "platform",
                },
                now.AddYears(-1),
                now.AddMinutes(-5)),
            new(
                "asset-db-01",
                "orders-db-primary",
                "database",
                "data-platform",
                AssetLifecycleState.Active,
                [new("cloud-resource-id", "/subscriptions/prod/sql/orders-db", true), new("dns", "orders-db.internal", false)],
                new Dictionary<string, string>
                {
                    ["environment"] = "production",
                    ["location"] = "canada-central",
                    ["operatingSystem"] = "PostgreSQL 17",
                    ["encryption"] = "enabled",
                    ["owner"] = "data-platform",
                },
                now.AddYears(-2),
                now.AddMinutes(-2)),
            new(
                "asset-worker-07",
                "inventory-worker-07",
                "container-host",
                "inventory",
                AssetLifecycleState.Active,
                [new("hostname", "inventory-worker-07", false)],
                new Dictionary<string, string>
                {
                    ["environment"] = "staging",
                    ["location"] = "east-us",
                    ["operatingSystem"] = "Flatcar 4152",
                    ["owner"] = "inventory",
                },
                now.AddMonths(-5),
                now.AddDays(-8)),
        ];
    }

    public static AssetEvidence CreateWebServerEvidence(DateTimeOffset now)
    {
        return new(
            "evidence-agent-web-01",
            AssetEvidenceSource.Agent,
            "agent-fleet-prod",
            "WEB-PROD-01",
            "virtual-machine",
            [new("cloud-resource-id", "/subscriptions/prod/vm/web-prod-01", true), new("hostname", "web-prod-01", false)],
            new Dictionary<string, string>
            {
                ["environment"] = "production",
                ["location"] = "canada-central",
                ["operatingSystem"] = "Ubuntu 26.04",
                ["encryption"] = "enabled",
                ["owner"] = "platform",
                ["kernel"] = "6.14",
            },
            now,
            0.95d);
    }

    public static DesiredAssetState CreateWebServerDesiredState()
    {
        return new(
            "asset-web-01",
            new Dictionary<string, string>
            {
                ["environment"] = "production",
                ["location"] = "canada-central",
                ["operatingSystem"] = "Ubuntu 24.04",
                ["encryption"] = "enabled",
                ["owner"] = "platform",
                ["firewall"] = "enabled",
            });
    }

    public static IReadOnlyList<ReportRow> CreateOperationalRows(DateTimeOffset now)
    {
        return
        [
            CreateReportRow("INC-1042", "incident", "production", "Sev1", "open", 48m, 1m, now.AddMinutes(-48)),
            CreateReportRow("INC-1041", "incident", "production", "Sev2", "open", 120m, 1m, now.AddHours(-2)),
            CreateReportRow("INC-1038", "incident", "shared", "Sev3", "mitigated", 480m, 1m, now.AddHours(-8)),
            CreateReportRow("asset-web-01", "asset", "production", "high", "active", 5m, 1m, now.AddMinutes(-5)),
            CreateReportRow("asset-db-01", "asset", "production", "critical", "active", 2m, 1m, now.AddMinutes(-2)),
            CreateReportRow("asset-worker-07", "asset", "staging", "medium", "stale", 11520m, 1m, now.AddDays(-8)),
        ];
    }

    public static IReadOnlyList<ReportDefinition> CreateReports()
    {
        return
        [
            new(
                "production-by-kind",
                "Production objects by kind",
                "environment == 'production'",
                ["kind"],
                [],
                [
                    new("count", null, ReportAggregation.Count, "Object count"),
                    new("averageAge", "ageMinutes", ReportAggregation.Average, "Average age (minutes)"),
                ],
                [new("count", ReportSortDirection.Descending)]),
            new(
                "open-incidents",
                "Open incident detail",
                "kind == 'incident' and status != 'resolved'",
                [],
                [
                    new("id", "id", "Identifier"),
                    new("severity", "severity", "Severity"),
                    new("ageHours", "ageMinutes / 60", "Age (hours)"),
                ],
                [],
                [new("ageHours", ReportSortDirection.Descending)]),
        ];
    }

    private static ReportRow CreateReportRow(
        string id,
        string kind,
        string environment,
        string severity,
        string status,
        decimal ageMinutes,
        decimal count,
        DateTimeOffset observedAt)
    {
        return new(
            id,
            new Dictionary<string, object?>
            {
                ["id"] = id,
                ["kind"] = kind,
                ["environment"] = environment,
                ["severity"] = severity,
                ["status"] = status,
                ["ageMinutes"] = ageMinutes,
                ["count"] = count,
                ["observedAt"] = observedAt,
            });
    }
}
