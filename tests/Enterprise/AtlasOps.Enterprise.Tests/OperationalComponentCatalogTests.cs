namespace AtlasOps.Enterprise.Tests;

using AtlasOps.Enterprise.Avalonia.Common;

[TestClass]
public sealed class OperationalComponentCatalogTests
{
    [TestMethod]
    public void Components_ExposeDistinctPackageBackedOperationalControls()
    {
        IReadOnlyList<OperationalComponentDescriptor> components = OperationalComponentCatalog.Components;

        Assert.HasCount(5, components);
        Assert.AreEqual(components.Count, components.Select(static item => item.Id).Distinct(StringComparer.Ordinal).Count());
        Assert.AreEqual(components.Count, components.Select(static item => item.PackageId).Distinct(StringComparer.Ordinal).Count());
        Assert.IsTrue(components.All(static item => typeof(global::Avalonia.Controls.Control).IsAssignableFrom(item.ControlType)));
    }
}
