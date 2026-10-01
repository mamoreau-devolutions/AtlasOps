namespace AtlasOps.Integrations.Tests;

using AtlasOps.Integrations.Identity.Contracts;
using AtlasOps.Integrations.Identity.Core;

[TestClass]
public sealed class IdentityReconciliationServiceTests
{
    private static readonly DateTimeOffset Now = new(2026, 5, 6, 7, 8, 9, TimeSpan.Zero);

    [TestMethod]
    public void CreatePlan_MismatchedSubject_ReturnsDiagnosticWithoutChanges()
    {
        IdentityReconciliationService service = new();

        IdentityReconciliationPlan result = service.CreatePlan(
            Subject("one", true, "1"),
            Subject("two", true, "1"),
            [],
            [],
            Now);

        Assert.IsEmpty(result.Changes);
        CollectionAssert.AreEqual(
            new[] { "Current and desired subjects must have the same provider identifier." },
            result.Diagnostics.ToArray());
    }

    [TestMethod]
    public void CreatePlan_DisableRevokeUpdateAndAdd_AreDeterministicallyOrdered()
    {
        IdentityReconciliationService service = new();
        IdentitySubject current = Subject("subject", true, "1") with
        {
            Attributes = new Dictionary<string, string> { ["name"] = "old" },
        };
        IdentitySubject desired = Subject("subject", false, "2") with
        {
            Attributes = new Dictionary<string, string> { ["name"] = "new" },
        };
        AccessGrant existing = Grant("z-resource", "Reader");
        AccessGrant desiredNew = Grant("a-resource", "Writer");

        IdentityReconciliationPlan result = service.CreatePlan(
            current,
            desired,
            [existing],
            [desiredNew],
            Now);

        CollectionAssert.AreEqual(
            new[]
            {
                IdentityChangeKind.DisableSubject,
                IdentityChangeKind.RevokeGrant,
                IdentityChangeKind.UpdateSubject,
                IdentityChangeKind.AddGrant,
            },
            result.Changes.Select(static change => change.Kind).ToArray());
    }

    [TestMethod]
    public void CreatePlan_ExpiredGrantRevokesAndCaseInsensitiveExistingGrantDoesNotAdd()
    {
        IdentityReconciliationService service = new();
        AccessGrant expired = Grant("expired", "Reader") with { ExpiresAt = Now };
        AccessGrant existing = Grant("Resource", "Admin");
        AccessGrant sameDifferentCase = Grant("resource", "admin");

        IdentityReconciliationPlan result = service.CreatePlan(
            Subject("subject", true, "1"),
            Subject("subject", true, "1"),
            [expired, existing],
            [sameDifferentCase],
            Now);

        Assert.HasCount(1, result.Changes);
        Assert.AreEqual(IdentityChangeKind.RevokeGrant, result.Changes[0].Kind);
        Assert.AreEqual("Grant expired.", result.Changes[0].Reason);
    }

    [TestMethod]
    public void CreatePlan_DisabledToEnabled_AddsGrantBeforeEnable()
    {
        IdentityReconciliationService service = new();

        IdentityReconciliationPlan result = service.CreatePlan(
            Subject("subject", false, "1"),
            Subject("subject", true, "1"),
            [],
            [Grant("resource", "reader")],
            Now);

        CollectionAssert.AreEqual(
            new[] { IdentityChangeKind.AddGrant, IdentityChangeKind.EnableSubject },
            result.Changes.Select(static change => change.Kind).ToArray());
    }

    [TestMethod]
    public void CreatePlan_EquivalentSubjectAndGrants_ProducesNoChanges()
    {
        IdentityReconciliationService service = new();
        IdentitySubject subject = Subject("subject", true, "1");
        AccessGrant grant = Grant("resource", "reader");

        IdentityReconciliationPlan result = service.CreatePlan(
            subject,
            subject,
            [grant],
            [grant],
            Now);

        Assert.IsEmpty(result.Changes);
        Assert.IsEmpty(result.Diagnostics);
    }

    private static IdentitySubject Subject(string id, bool enabled, string revision)
    {
        return new IdentitySubject("provider", id, "user", enabled, revision, new Dictionary<string, string>());
    }

    private static AccessGrant Grant(string resource, string role)
    {
        return new AccessGrant("subject", resource, role, Now, null, "test");
    }
}
