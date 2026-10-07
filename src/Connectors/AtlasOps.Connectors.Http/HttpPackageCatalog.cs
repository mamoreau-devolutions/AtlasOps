namespace AtlasOps.Connectors.Http;

using System.Text.Json.Nodes;

using HtmlAgilityPack;

using Json.Pointer;
using Json.Schema;

using Microsoft.AspNetCore.WebUtilities;

using Nager.PublicSuffix;

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
        new("json-nodes", typeof(JsonNode), "JSON token processing"),
        new("json-schema", typeof(JsonSchema), "JSON schema validation"),
        new("json-pointer", typeof(JsonPointer), "JSON schema diagnostic locations"),
    ];
}
