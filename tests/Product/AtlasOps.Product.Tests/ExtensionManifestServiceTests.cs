namespace AtlasOps.Product.Tests;

using AtlasOps.Extensions.Contracts;
using AtlasOps.Extensions.Runtime;

[TestClass]
public sealed class ExtensionManifestServiceTests
{
    [TestMethod]
    public void Validate_WellFormedManifest_IsValid()
    {
        ExtensionManifestService service = new();
        ExtensionManifest manifest = Manifest() with
        {
            Permissions =
            [
                new ExtensionPermission("read", "Read", true, "Required to read data."),
            ],
            Capabilities =
            [
                new ExtensionCapability("sync", "1.0", ["import"]),
            ],
            Migrations =
            [
                new ExtensionConfigurationMigration(1, 2, "one-to-two", [], []),
                new ExtensionConfigurationMigration(2, 3, "two-to-three", [], []),
            ],
        };

        ExtensionValidationResult result = service.Validate(manifest, new HashSet<string>(["read"]));

        Assert.IsTrue(result.Valid);
        Assert.IsEmpty(result.Diagnostics);
    }

    [TestMethod]
    public void Validate_InvalidIdentityVersionPermissionsAndCapabilities_ReturnDiagnostics()
    {
        ExtensionManifestService service = new();
        ExtensionManifest manifest = Manifest() with
        {
            Id = "bad/id",
            Version = "0.9",
            Permissions =
            [
                new ExtensionPermission("unknown", "Unknown", true, ""),
                new ExtensionPermission("UNKNOWN", "Duplicate", false, ""),
            ],
            Capabilities =
            [
                new ExtensionCapability("empty", "1.0", []),
                new ExtensionCapability("EMPTY", "1.0", ["operation"]),
            ],
        };

        ExtensionValidationResult result = service.Validate(manifest, new HashSet<string>());

        Assert.IsFalse(result.Valid);
        Assert.Contains("Extension ID must use letters, digits, periods, and hyphens.", result.Diagnostics);
        Assert.Contains("Extension version must be a semantic version with major version one or later.", result.Diagnostics);
        Assert.Contains("Permission 'unknown' is not recognized.", result.Diagnostics);
        Assert.Contains("Permission 'UNKNOWN' is duplicated.", result.Diagnostics);
        Assert.Contains("Sensitive permission 'unknown' requires a justification.", result.Diagnostics);
        Assert.Contains("Capability 'empty' must expose at least one operation.", result.Diagnostics);
        Assert.Contains("Capability 'EMPTY' is duplicated.", result.Diagnostics);
    }

    [TestMethod]
    public void Validate_InvalidMigrationChain_DetectsDuplicateStepAndGap()
    {
        ExtensionManifestService service = new();
        ExtensionManifest manifest = Manifest() with
        {
            Migrations =
            [
                new ExtensionConfigurationMigration(1, 3, "migrate", [], []),
                new ExtensionConfigurationMigration(4, 5, "MIGRATE", [], []),
            ],
        };

        ExtensionValidationResult result = service.Validate(manifest, new HashSet<string>());

        Assert.Contains("Migration 'migrate' must advance exactly one version.", result.Diagnostics);
        Assert.Contains("Migration 'MIGRATE' is duplicated.", result.Diagnostics);
        Assert.Contains("Migration chain skips version 3.", result.Diagnostics);
    }

    [TestMethod]
    public void EvaluateCompatibility_AtInclusiveBounds_IsCompatible()
    {
        ExtensionManifestService service = new();
        ExtensionManifest manifest = Manifest() with
        {
            MinimumHostVersion = "2.0",
            MaximumHostVersion = "3.0",
        };

        ExtensionCompatibilityResult minimum = service.EvaluateCompatibility(manifest, new Version(2, 0));
        ExtensionCompatibilityResult maximum = service.EvaluateCompatibility(manifest, new Version(3, 0));

        Assert.IsTrue(minimum.Compatible);
        Assert.IsTrue(maximum.Compatible);
        Assert.IsEmpty(minimum.Diagnostics);
    }

    [TestMethod]
    public void EvaluateCompatibility_InvalidAndOutsideBounds_ReturnDiagnostics()
    {
        ExtensionManifestService service = new();
        ExtensionCompatibilityResult invalid = service.EvaluateCompatibility(
            Manifest() with { MinimumHostVersion = "bad", MaximumHostVersion = "also-bad" },
            new Version(2, 0));
        ExtensionManifest bounded = Manifest() with
        {
            MinimumHostVersion = "2.0",
            MaximumHostVersion = "3.0",
        };
        ExtensionCompatibilityResult old = service.EvaluateCompatibility(bounded, new Version(1, 9));
        ExtensionCompatibilityResult newer = service.EvaluateCompatibility(bounded, new Version(3, 1));

        CollectionAssert.AreEqual(
            new[] { "Minimum host version is invalid.", "Maximum host version is invalid." },
            invalid.Diagnostics.ToArray());
        StringAssert.Contains(old.Diagnostics.Single(), "older than required");
        StringAssert.Contains(newer.Diagnostics.Single(), "newer than supported");
    }

    internal static ExtensionManifest Manifest()
    {
        return new ExtensionManifest(
            "atlas.extension",
            "Atlas Extension",
            "1.0",
            "1.0",
            "9.0",
            [],
            [],
            []);
    }
}
