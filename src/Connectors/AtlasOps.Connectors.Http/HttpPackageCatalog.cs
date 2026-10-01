namespace AtlasOps.Connectors.Http;

using HtmlAgilityPack;

using Microsoft.AspNetCore.WebUtilities;

using Nager.PublicSuffix;

using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

using NJsonSchema;

using RestSharp;

public sealed record HttpPackageDescriptor(string Id, Type PrimaryType, string Capability);

public static class HttpPackageCatalog
{
    public static IReadOnlyList<string> OpenApiPackages { get; } =
    [
        "NSwag.Annotations",
        "NSwag.AspNetCore",
    ];

    public static IReadOnlyList<HttpPackageDescriptor> Packages { get; } =
    [
        new("restsharp", typeof(RestRequest), "REST request construction"),
        new("html-agility-pack", typeof(HtmlDocument), "HTML response inspection"),
        new("web-utilities", typeof(QueryHelpers), "Query-string encoding"),
        new("public-suffix", typeof(DomainParser), "Registrable domain analysis"),
        new("newtonsoft-json", typeof(JToken), "JSON token processing"),
        new("newtonsoft-schema", typeof(JSchema), "JSON schema validation"),
        new("njsonschema", typeof(NJsonSchema.JsonSchema), "JSON schema generation and validation"),
    ];
}
