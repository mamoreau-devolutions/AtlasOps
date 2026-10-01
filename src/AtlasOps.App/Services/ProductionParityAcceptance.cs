namespace AtlasOps.App.Services;

using AtlasOps.Extensions.Runtime;
using AtlasOps.Integrations.Databases.Core;
using AtlasOps.Integrations.GitProviders.Core;
using AtlasOps.Integrations.Identity.Core;
using AtlasOps.Integrations.Kubernetes.Core;
using AtlasOps.Integrations.Mail.Core;
using AtlasOps.Integrations.Messaging.Core;
using AtlasOps.Integrations.Monitoring.Core;
using AtlasOps.Integrations.ObjectStorage.Core;
using AtlasOps.Integrations.SecureShell.Core;
using AtlasOps.Integrations.ServiceManagement.Core;
using AtlasOps.Integrations.Webhooks.Core;
using AtlasOps.Integrations.WorkTracking.Core;
using AtlasOps.Operations.Contracts;
using AtlasOps.Operations.Runtime;
using AtlasOps.Product.Localization;
using AtlasOps.Product.Migrations;
using AtlasOps.Product.Release;
using AtlasOps.Product.Support;

public static class ProductionParityAcceptance
{
    public static IReadOnlyList<string> Validate()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        DurableJob job = new(
            Guid.NewGuid(),
            new OperationEnvelope(
                Guid.NewGuid(),
                "acceptance",
                "validate",
                now,
                "credential-reference",
                new Dictionary<string, string>()),
            DurableJobStatus.Pending,
            0,
            3,
            now,
            now,
            now,
            0,
            null);
        JobMutationResult leased = OperationStateMachine.Transition(
            job,
            DurableJobStatus.Leased,
            now,
            now,
            "Acceptance lease");
        if (!leased.Succeeded || leased.Job is null || leased.Job.Revision != 1)
        {
            throw new InvalidOperationException("Durable operations acceptance failed.");
        }

        Type[] integrationServices =
        [
            typeof(GitProviderService),
            typeof(WorkTrackingService),
            typeof(KubernetesReconciliationService),
            typeof(SecureShellService),
            typeof(DatabasePlanningService),
            typeof(ObjectStoragePlanningService),
            typeof(MessageDeliveryService),
            typeof(MailPlanningService),
            typeof(WebhookSecurityService),
            typeof(IdentityReconciliationService),
            typeof(MonitoringAnalysisService),
            typeof(ServiceLevelService),
        ];
        if (integrationServices.Select(static type => type.Assembly).Distinct().Count() != 12)
        {
            throw new InvalidOperationException("Integration-family composition acceptance failed.");
        }

        Type[] productServices =
        [
            typeof(ExtensionManifestService),
            typeof(ExtensionMigrationService),
            typeof(LocalizationCatalog),
            typeof(SettingsMigrationService),
            typeof(ReleaseChannelService),
            typeof(SupportBundleService),
        ];
        if (productServices.Any(static type => type.IsAbstract))
        {
            throw new InvalidOperationException("Product-operations composition acceptance failed.");
        }

        return
        [
            "durable operations",
            .. integrationServices.Select(static type => type.Namespace ?? type.Name),
            .. productServices.Select(static type => type.Namespace ?? type.Name),
        ];
    }
}
