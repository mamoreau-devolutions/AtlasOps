namespace AtlasOps.Architecture.Tests;

using System.Xml.Linq;

[TestClass]
public sealed class ModularArchitectureTests
{
    [TestMethod]
    public void Solution_ContainsExactlyOneHundredFiftyFourProjects()
    {
        string solution = Path.Combine(FindAtlasOpsRoot(), "AtlasOps.sln");
        string[] projects = File.ReadLines(solution)
            .Where(static line => line.StartsWith("Project(", StringComparison.Ordinal)
                && line.Contains(".csproj", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        Assert.HasCount(154, projects);
        Assert.AreEqual(154, projects.Distinct(StringComparer.OrdinalIgnoreCase).Count());
    }

    [TestMethod]
    public void DomainTriplets_KeepContractsIndependentAndCoreInwardFacing()
    {
        string modularRoot = Path.Combine(FindAtlasOpsRoot(), "src", "Modular");
        string modulesRoot = Path.Combine(modularRoot, "Modules");
        string[] domainDirectories = Directory.GetDirectories(modulesRoot);

        Assert.HasCount(18, domainDirectories);
        foreach (string domainDirectory in domainDirectories)
        {
            string domain = Path.GetFileName(domainDirectory);
            string contracts = SingleProject(domainDirectory, $".{domain}.Contracts");
            string core = SingleProject(domainDirectory, $".{domain}.Core");
            string avalonia = SingleProject(domainDirectory, $".{domain}.Avalonia");

            Assert.IsEmpty(ProjectReferences(contracts), $"{domain} contracts must have no project dependencies.");

            string[] coreReferences = ProjectReferences(core);
            Assert.IsTrue(coreReferences.Any(reference => reference.EndsWith(
                $"AtlasOps.Modules.{domain}.Contracts.csproj", StringComparison.OrdinalIgnoreCase)));
            Assert.IsFalse(coreReferences.Any(IsOutwardReference), $"{domain} core has an outward reference.");

            string[] avaloniaReferences = ProjectReferences(avalonia);
            Assert.IsTrue(avaloniaReferences.Any(reference => reference.EndsWith(
                $"AtlasOps.Modules.{domain}.Core.csproj", StringComparison.OrdinalIgnoreCase)));
            Assert.IsTrue(avaloniaReferences.Any(reference => reference.EndsWith(
                $"AtlasOps.Modules.{domain}.Contracts.csproj", StringComparison.OrdinalIgnoreCase)));
        }
    }

    [TestMethod]
    public void Adapters_DoNotReferenceAvaloniaOrConcretePeerAdapters()
    {
        string adaptersRoot = Path.Combine(FindAtlasOpsRoot(), "src", "Modular", "Adapters");
        string[] projects = Directory.GetFiles(adaptersRoot, "*.csproj", SearchOption.AllDirectories);

        Assert.HasCount(10, projects);
        foreach (string project in projects)
        {
            string[] references = ProjectReferences(project);
            Assert.IsFalse(references.Any(static reference =>
                reference.Contains(".Avalonia", StringComparison.OrdinalIgnoreCase)
                || reference.Contains(@"\Adapters\", StringComparison.OrdinalIgnoreCase)),
                $"{Path.GetFileName(project)} references an outward UI or peer adapter.");
        }
    }

    private static bool IsOutwardReference(string reference) =>
        reference.Contains(".Avalonia", StringComparison.OrdinalIgnoreCase)
        || reference.Contains(@"\Adapters\", StringComparison.OrdinalIgnoreCase)
        || reference.Contains(@"\Tools\", StringComparison.OrdinalIgnoreCase);

    private static string SingleProject(string directory, string suffix)
    {
        string[] projects = Directory.GetFiles(directory, $"*{suffix}.csproj", SearchOption.AllDirectories);
        Assert.HasCount(1, projects, $"Expected one {suffix} project below {directory}.");
        return projects[0];
    }

    private static string[] ProjectReferences(string project) =>
        XDocument.Load(project)
            .Descendants("ProjectReference")
            .Select(static element => (string?)element.Attribute("Include"))
            .Where(static include => include is not null)
            .Cast<string>()
            .ToArray();

    private static string FindAtlasOpsRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);
        while (directory is not null)
        {
            string candidate = Path.Combine(directory.FullName, "AtlasOps.sln");
            if (File.Exists(candidate))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        Assert.Fail("Could not locate AtlasOps.sln from the test output directory.");
        return string.Empty;
    }
}