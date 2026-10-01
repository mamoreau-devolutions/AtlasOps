namespace AtlasOps.Tests;

using System.Text.Json;

using AtlasOps.Core;
using AtlasOps.Core.Generated;

[TestClass]
public sealed class GeneratedModelTests
{
    public static IEnumerable<object[]> GetModelCases() => ModelTestData.GetCases();

    [TestMethod]
    [DynamicData(nameof(GetModelCases))]
    public void Catalog_HasExpectedDescriptor_ForEveryGeneratedModel(ModelCase modelCase)
    {
        Assert.HasCount(20, AtlasOpsGeneratedModelCatalog.Models);

        AtlasOpsModelDescriptor descriptor = AtlasOpsGeneratedModelCatalog.Models
            .Single(item => item.TypeName == modelCase.Type.Name);

        Assert.AreEqual(modelCase.Category, descriptor.Category);
        Assert.AreEqual(modelCase.DisplayName, descriptor.DisplayName);
        Assert.HasCount(modelCase.Type.GetProperties().Length, descriptor.Fields);
        Assert.IsFalse(descriptor.Fields.Single(field => field.Name == "Id").IsEditable);
        Assert.IsFalse(descriptor.Fields.Single(field => field.Name == "UpdatedAt").IsEditable);

        string[] expectedEditable = ModelTestData.EditableProperties(modelCase.Type)
            .Select(static property => property.Name)
            .ToArray();
        string[] actualEditable = descriptor.Fields
            .Where(static field => field.IsEditable)
            .Select(static field => field.Name)
            .OrderBy(static name => name, StringComparer.Ordinal)
            .ToArray();
        CollectionAssert.AreEqual(expectedEditable, actualEditable);
        foreach (AtlasOpsFieldDescriptor field in descriptor.Fields)
        {
            Type propertyType = modelCase.Type.GetProperty(field.Name)!.PropertyType;
            Assert.AreEqual(ExpectedDescriptorTypeName(propertyType), field.TypeName);
        }
    }

    [TestMethod]
    [DynamicData(nameof(GetModelCases))]
    public void WorkspaceCreateAndEditorFactory_CreateExactGeneratedTypes(ModelCase modelCase)
    {
        IAtlasOpsEntity entity = AtlasOpsGeneratedWorkspaceManager.Create(modelCase.Type.Name);
        IGeneratedEditorViewModel editor = AtlasOpsGeneratedEditorFactory.Create(entity);

        Assert.AreEqual(modelCase.Type, entity.GetType());
        Assert.IsFalse(string.IsNullOrWhiteSpace(entity.Id));
        Assert.AreSame(entity, editor.Model);
        Assert.AreEqual(modelCase.Type.Name, editor.ModelType);
        Assert.AreEqual(modelCase.DisplayName, editor.DisplayName);
        Assert.AreEqual(modelCase.Category, editor.EditorCategory);
        Assert.AreEqual($"{modelCase.Type.Name}EditorViewModel", editor.GetType().Name);
        Assert.HasCount(ModelTestData.EditableProperties(modelCase.Type).Count, editor.Fields);
    }

    [TestMethod]
    [DynamicData(nameof(GetModelCases))]
    public void Serializer_RoundTripsEveryGeneratedModel_ByDiscriminatorAndGenericApi(ModelCase modelCase)
    {
        IAtlasOpsEntity expected = ModelTestData.CreatePopulated(modelCase);

        string payload = AtlasOpsGeneratedSerializer.Serialize(expected);
        IAtlasOpsEntity byDiscriminator =
            AtlasOpsGeneratedSerializer.Deserialize(modelCase.Type.Name, payload);
        IAtlasOpsEntity byGeneric = DeserializeGeneric(modelCase.Type, payload);

        StringAssert.Contains(payload, $"\"id\": \"{expected.Id}\"");
        ModelTestData.AssertAllPropertiesEqual(expected, byDiscriminator);
        ModelTestData.AssertAllPropertiesEqual(expected, byGeneric);
    }

    [TestMethod]
    public void Serializer_RejectsUnknownDiscriminatorUnsupportedEntityAndNullPayload()
    {
        Assert.ThrowsExactly<NotSupportedException>(
            () => AtlasOpsGeneratedSerializer.Deserialize("UnknownModel", "{}"));
        Assert.ThrowsExactly<NotSupportedException>(
            () => AtlasOpsGeneratedSerializer.Deserialize("atlasopsproject", "{}"));
        Assert.ThrowsExactly<NotSupportedException>(
            () => AtlasOpsGeneratedSerializer.Serialize(new UnsupportedEntity()));
        Assert.ThrowsExactly<JsonException>(
            () => AtlasOpsGeneratedSerializer.Deserialize("AtlasOpsProject", "null"));
        Assert.ThrowsExactly<JsonException>(
            () => AtlasOpsGeneratedSerializer.Deserialize<AtlasOpsProject>("null"));
        Assert.ThrowsExactly<JsonException>(
            () => AtlasOpsGeneratedSerializer.Deserialize("AtlasOpsProject", "{broken"));
        Assert.ThrowsExactly<JsonException>(
            () => AtlasOpsGeneratedSerializer.Deserialize<AtlasOpsProject>("{broken"));
    }

    [TestMethod]
    public void GeneratedFactories_RejectUnsupportedModels()
    {
        UnsupportedEntity unsupported = new();

        Assert.ThrowsExactly<NotSupportedException>(
            () => AtlasOpsGeneratedWorkspaceManager.Create(nameof(UnsupportedEntity)));
        Assert.ThrowsExactly<NotSupportedException>(
            () => AtlasOpsGeneratedEditorFactory.Create(unsupported));
        Assert.ThrowsExactly<NotSupportedException>(
            () => AtlasOpsGeneratedWorkspaceManager.Summarize(unsupported));
    }

    [TestMethod]
    public void Summarize_ReturnsConcreteMetadataAndSearchContent()
    {
        AtlasOpsProject project = new()
        {
            Id = "project-7",
            Name = "Atlas launch",
            Notes = "Ship safely",
            Owner = "Platform",
            Priority = 7,
            Stage = "Delivery",
            IsPinned = true,
            UpdatedAt = ModelTestData.FixedTimestamp,
        };

        AtlasOpsEntitySummary summary = AtlasOpsGeneratedWorkspaceManager.Summarize(project);

        Assert.AreSame(project, summary.Entity);
        Assert.AreEqual(nameof(AtlasOpsProject), summary.TypeName);
        Assert.AreEqual("Workspace", summary.Category);
        Assert.AreEqual("Projects", summary.DisplayName);
        Assert.AreEqual("Atlas launch", summary.PrimaryText);
        Assert.AreEqual("Ship safely", summary.SecondaryText);
        StringAssert.Contains(summary.SearchText, "project-7");
        StringAssert.Contains(summary.SearchText, "Platform");
        StringAssert.Contains(summary.SearchText, "Delivery");
        StringAssert.Contains(summary.SearchText, "7");
    }

    private static IAtlasOpsEntity DeserializeGeneric(Type type, string payload)
    {
        return (IAtlasOpsEntity)typeof(AtlasOpsGeneratedSerializer)
            .GetMethods()
            .Single(method => method.Name == nameof(AtlasOpsGeneratedSerializer.Deserialize) &&
                method.IsGenericMethodDefinition)
            .MakeGenericMethod(type)
            .Invoke(null, [payload])!;
    }

    private static string ExpectedDescriptorTypeName(Type type)
    {
        return type == typeof(string)
            ? "string"
            : type == typeof(bool)
                ? "bool"
                : type == typeof(int)
                    ? "int"
                    : type == typeof(long)
                        ? "long"
                        : type == typeof(double)
                            ? "double"
                            : type == typeof(decimal)
                                ? "decimal"
                                : type == typeof(DateTimeOffset)
                                    ? "global::System.DateTimeOffset"
                                    : throw new InvalidOperationException(
                                        $"No descriptor type mapping exists for {type.FullName}.");
    }
}