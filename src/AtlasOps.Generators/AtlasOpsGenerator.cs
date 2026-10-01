namespace AtlasOps.Generators;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

[Generator]
public sealed class AtlasOpsGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<ModelInfo> modelProvider = context.SyntaxProvider.ForAttributeWithMetadataName(
            "AtlasOps.Core.GenerateAtlasOpsModelAttribute",
            static (node, _) => node is ClassDeclarationSyntax,
            static (syntaxContext, _) => GetModel(syntaxContext));

        context.RegisterSourceOutput(modelProvider.Collect(), static (sourceContext, models) =>
        {
            if (models.IsDefaultOrEmpty)
            {
                return;
            }

            ImmutableArray<ModelInfo> sortedModels = models
                .OrderBy(static model => model.Category, StringComparer.Ordinal)
                .ThenBy(static model => model.TypeName, StringComparer.Ordinal)
                .ToImmutableArray();

            sourceContext.AddSource(
                "AtlasOps.Generated.g.cs",
                SourceText.From(GenerateSource(sortedModels), Encoding.UTF8));
        });
    }

    private static ModelInfo GetModel(GeneratorAttributeSyntaxContext context)
    {
        INamedTypeSymbol type = (INamedTypeSymbol)context.TargetSymbol;
        AttributeData attribute = context.Attributes[0];
        string category = attribute.ConstructorArguments[0].Value as string ?? "General";
        string displayName = attribute.ConstructorArguments[1].Value as string ?? type.Name;
        ImmutableArray<PropertyInfo> properties = type.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(static property => property.DeclaredAccessibility == Accessibility.Public && !property.IsStatic)
            .Select(static property => new PropertyInfo(
                property.Name,
                property.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                property.SetMethod is not null &&
                property.Name != "Id" &&
                property.Name != "UpdatedAt"))
            .OrderBy(static property => property.Name, StringComparer.Ordinal)
            .ToImmutableArray();

        return new ModelInfo(
            type.Name,
            type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            category,
            displayName,
            properties);
    }

    private static string GenerateSource(ImmutableArray<ModelInfo> models)
    {
        StringBuilder builder = new();
        builder.AppendLine("namespace AtlasOps.Core.Generated;");
        builder.AppendLine();
        builder.AppendLine("using System.Text.Json;");
        builder.AppendLine("using AtlasOps.Core;");
        builder.AppendLine();
        AppendCatalog(builder, models);
        AppendSerializer(builder, models);
        AppendWorkspace(builder, models);
        AppendEditorFactory(builder, models);

        foreach (ModelInfo model in models)
        {
            AppendEditorViewModel(builder, model);
        }

        return builder.ToString();
    }

    private static void AppendCatalog(StringBuilder builder, ImmutableArray<ModelInfo> models)
    {
        builder.AppendLine("public static class AtlasOpsGeneratedModelCatalog");
        builder.AppendLine("{");
        builder.AppendLine("    public static IReadOnlyList<AtlasOpsModelDescriptor> Models { get; } =");
        builder.AppendLine("    [");

        foreach (ModelInfo model in models)
        {
            builder.AppendLine($"        new(\"{Escape(model.TypeName)}\", \"{Escape(model.Category)}\", \"{Escape(model.DisplayName)}\",");
            builder.AppendLine("        [");

            foreach (PropertyInfo property in model.Properties)
            {
                string editable = property.IsEditable ? "true" : "false";
                builder.AppendLine($"            new(\"{Escape(property.Name)}\", \"{Escape(property.TypeName)}\", {editable}),");
            }

            builder.AppendLine("        ]),");
        }

        builder.AppendLine("    ];");
        builder.AppendLine("}");
        builder.AppendLine();
    }

    private static void AppendSerializer(StringBuilder builder, ImmutableArray<ModelInfo> models)
    {
        builder.AppendLine("public static class AtlasOpsGeneratedSerializer");
        builder.AppendLine("{");
        builder.AppendLine("    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)");
        builder.AppendLine("    {");
        builder.AppendLine("        WriteIndented = true,");
        builder.AppendLine("    };");
        builder.AppendLine();
        builder.AppendLine("    public static string Serialize(IAtlasOpsEntity model)");
        builder.AppendLine("    {");
        builder.AppendLine("        return model switch");
        builder.AppendLine("        {");

        foreach (ModelInfo model in models)
        {
            builder.AppendLine($"            {model.QualifiedTypeName} value => JsonSerializer.Serialize(value, Options),");
        }

        builder.AppendLine("            _ => throw new NotSupportedException($\"Unsupported AtlasOps model: {model.GetType().FullName}\"),");
        builder.AppendLine("        };");
        builder.AppendLine("    }");
        builder.AppendLine();
        builder.AppendLine("    public static IAtlasOpsEntity Deserialize(string modelType, string payload)");
        builder.AppendLine("    {");
        builder.AppendLine("        return modelType switch");
        builder.AppendLine("        {");

        foreach (ModelInfo model in models)
        {
            builder.AppendLine($"            \"{Escape(model.TypeName)}\" => JsonSerializer.Deserialize<{model.QualifiedTypeName}>(payload, Options)");
            builder.AppendLine($"                ?? throw new JsonException(\"Unable to deserialize {Escape(model.TypeName)}.\"),");
        }

        builder.AppendLine("            _ => throw new NotSupportedException($\"Unsupported AtlasOps model type: {modelType}\"),");
        builder.AppendLine("        };");
        builder.AppendLine("    }");
        builder.AppendLine();
        builder.AppendLine("    public static TModel Deserialize<TModel>(string payload)");
        builder.AppendLine("        where TModel : class, IAtlasOpsEntity");
        builder.AppendLine("    {");
        builder.AppendLine("        return JsonSerializer.Deserialize<TModel>(payload, Options)");
        builder.AppendLine("            ?? throw new JsonException($\"Unable to deserialize {typeof(TModel).Name}.\");");
        builder.AppendLine("    }");
        builder.AppendLine("}");
        builder.AppendLine();
    }

    private static void AppendEditorFactory(StringBuilder builder, ImmutableArray<ModelInfo> models)
    {
        builder.AppendLine("public static class AtlasOpsGeneratedEditorFactory");
        builder.AppendLine("{");
        builder.AppendLine("    public static IGeneratedEditorViewModel Create(IAtlasOpsEntity model)");
        builder.AppendLine("    {");
        builder.AppendLine("        return model switch");
        builder.AppendLine("        {");

        foreach (ModelInfo model in models)
        {
            builder.AppendLine($"            {model.QualifiedTypeName} value => new {model.TypeName}EditorViewModel(value),");
        }

        builder.AppendLine("            _ => throw new NotSupportedException($\"Unsupported AtlasOps editor model: {model.GetType().FullName}\"),");
        builder.AppendLine("        };");
        builder.AppendLine("    }");
        builder.AppendLine("}");
        builder.AppendLine();
    }

    private static void AppendWorkspace(StringBuilder builder, ImmutableArray<ModelInfo> models)
    {
        builder.AppendLine("public static class AtlasOpsGeneratedWorkspaceManager");
        builder.AppendLine("{");
        builder.AppendLine("    public static async Task<AtlasOpsGeneratedWorkspace> LoadAsync(");
        builder.AppendLine("        IAtlasOpsStore store,");
        builder.AppendLine("        CancellationToken cancellationToken = default)");
        builder.AppendLine("    {");
        builder.AppendLine("        List<IAtlasOpsEntity> entities = [];");

        foreach (ModelInfo model in models)
        {
            builder.AppendLine($"        entities.AddRange(await store.LoadAsync<{model.QualifiedTypeName}>(cancellationToken));");
        }

        builder.AppendLine("        return new AtlasOpsGeneratedWorkspace(entities);");
        builder.AppendLine("    }");
        builder.AppendLine();
        builder.AppendLine("    public static async Task SaveAsync(");
        builder.AppendLine("        IAtlasOpsStore store,");
        builder.AppendLine("        IEnumerable<IAtlasOpsEntity> entities,");
        builder.AppendLine("        CancellationToken cancellationToken = default)");
        builder.AppendLine("    {");
        builder.AppendLine("        List<IAtlasOpsEntity> materializedEntities = entities.ToList();");

        foreach (ModelInfo model in models)
        {
            builder.AppendLine($"        await store.SaveAsync(materializedEntities.OfType<{model.QualifiedTypeName}>(), cancellationToken);");
        }

        builder.AppendLine("    }");
        builder.AppendLine();
        builder.AppendLine("    public static IAtlasOpsEntity Create(string modelType)");
        builder.AppendLine("    {");
        builder.AppendLine("        return modelType switch");
        builder.AppendLine("        {");

        foreach (ModelInfo model in models)
        {
            builder.AppendLine($"            \"{Escape(model.TypeName)}\" => new {model.QualifiedTypeName}(),");
        }

        builder.AppendLine("            _ => throw new NotSupportedException($\"Unsupported AtlasOps model type: {modelType}\"),");
        builder.AppendLine("        };");
        builder.AppendLine("    }");
        builder.AppendLine();
        builder.AppendLine("    public static AtlasOpsEntitySummary Summarize(IAtlasOpsEntity entity)");
        builder.AppendLine("    {");
        builder.AppendLine("        return entity switch");
        builder.AppendLine("        {");

        foreach (ModelInfo model in models)
        {
            PropertyInfo[] textProperties = model.Properties
                .Where(static property => property.IsEditable &&
                    (property.TypeName == "global::System.String" || property.TypeName == "string"))
                .Take(2)
                .ToArray();
            string primaryText = textProperties.Length > 0
                ? $"value.{textProperties[0].Name}"
                : "value.Id";
            string secondaryText = textProperties.Length > 1
                ? $"value.{textProperties[1].Name}"
                : $"\"Updated \" + value.UpdatedAt.ToLocalTime().ToString(\"g\")";
            string searchText = string.Join(
                " + \" \" + ",
                model.Properties.Select(static property => $"value.{property.Name}.ToString()"));

            builder.AppendLine($"            {model.QualifiedTypeName} value => new(value, \"{Escape(model.TypeName)}\", \"{Escape(model.Category)}\", \"{Escape(model.DisplayName)}\",");
            builder.AppendLine($"                {primaryText}, {secondaryText}, {searchText}),");
        }

        builder.AppendLine("            _ => throw new NotSupportedException($\"Unsupported AtlasOps model: {entity.GetType().FullName}\"),");
        builder.AppendLine("        };");
        builder.AppendLine("    }");
        builder.AppendLine("}");
        builder.AppendLine();
    }

    private static void AppendEditorViewModel(StringBuilder builder, ModelInfo model)
    {
        builder.AppendLine($"public sealed class {model.TypeName}EditorViewModel");
        builder.AppendLine($"    : GeneratedEditorViewModelBase<{model.QualifiedTypeName}>");
        builder.AppendLine("{");
        builder.AppendLine($"    public {model.TypeName}EditorViewModel({model.QualifiedTypeName} model)");
        builder.AppendLine($"        : base(model, \"{Escape(model.DisplayName)}\", \"{Escape(model.Category)}\")");
        builder.AppendLine("    {");
        builder.AppendLine("        this.Fields =");
        builder.AppendLine("        [");

        foreach (PropertyInfo property in model.Properties.Where(static property => property.IsEditable))
        {
            string readExpression = GetReadExpression(property);
            string writeMethod = GetWriteMethod(property);
            builder.AppendLine($"            new(\"{Escape(property.Name)}\", \"{Escape(GetFriendlyTypeName(property.TypeName))}\",");
            builder.AppendLine($"                () => {readExpression},");
            builder.AppendLine($"                value => this.{writeMethod}(value, newValue => this.TypedModel.{property.Name} = newValue, nameof(this.{property.Name}))),");
        }

        builder.AppendLine("        ];");
        builder.AppendLine("    }");
        builder.AppendLine();
        builder.AppendLine("    public override IReadOnlyList<GeneratedFieldEditorViewModel> Fields { get; }");

        foreach (PropertyInfo property in model.Properties.Where(static property => property.IsEditable))
        {
            builder.AppendLine();
            builder.AppendLine($"    public {property.TypeName} {property.Name}");
            builder.AppendLine("    {");
            builder.AppendLine($"        get => this.TypedModel.{property.Name};");
            builder.AppendLine($"        set => this.SetProperty(this.TypedModel.{property.Name}, value, newValue => this.TypedModel.{property.Name} = newValue);");
            builder.AppendLine("    }");
        }

        builder.AppendLine("}");
        builder.AppendLine();
    }

    private static string GetReadExpression(PropertyInfo property)
    {
        return property.TypeName switch
        {
            "global::System.String" or "string" => $"this.TypedModel.{property.Name}",
            "global::System.DateTimeOffset" or "System.DateTimeOffset" => $"this.TypedModel.{property.Name}.ToString(\"O\")",
            _ => $"this.TypedModel.{property.Name}.ToString()",
        };
    }

    private static string GetWriteMethod(PropertyInfo property)
    {
        return property.TypeName switch
        {
            "global::System.String" or "string" => "SetString",
            "global::System.Boolean" or "bool" => "SetBoolean",
            "global::System.Int32" or "int" => "SetInteger",
            "global::System.Int64" or "long" => "SetLong",
            "global::System.Double" or "double" => "SetDouble",
            "global::System.Decimal" or "decimal" => "SetDecimal",
            "global::System.DateTimeOffset" or "System.DateTimeOffset" => "SetDateTimeOffset",
            _ => throw new InvalidOperationException($"No generated field adapter exists for {property.TypeName}."),
        };
    }

    private static string GetFriendlyTypeName(string typeName)
    {
        const string Prefix = "global::System.";
        return typeName.StartsWith(Prefix, StringComparison.Ordinal)
            ? typeName.Substring(Prefix.Length)
            : typeName;
    }

    private static string Escape(string value)
    {
        return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }

    private readonly struct ModelInfo
    {
        public ModelInfo(
            string typeName,
            string qualifiedTypeName,
            string category,
            string displayName,
            ImmutableArray<PropertyInfo> properties)
        {
            this.TypeName = typeName;
            this.QualifiedTypeName = qualifiedTypeName;
            this.Category = category;
            this.DisplayName = displayName;
            this.Properties = properties;
        }

        public string TypeName { get; }

        public string QualifiedTypeName { get; }

        public string Category { get; }

        public string DisplayName { get; }

        public ImmutableArray<PropertyInfo> Properties { get; }
    }

    private readonly struct PropertyInfo
    {
        public PropertyInfo(string name, string typeName, bool isEditable)
        {
            this.Name = name;
            this.TypeName = typeName;
            this.IsEditable = isEditable;
        }

        public string Name { get; }

        public string TypeName { get; }

        public bool IsEditable { get; }
    }
}