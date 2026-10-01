namespace AtlasOps.Connectors.Http;

using System.Text.RegularExpressions;

using HtmlAgilityPack;

using Microsoft.AspNetCore.WebUtilities;

using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

using NJsonSchema;

using RestSharp;

public sealed record HttpRequestPlan(
    Uri Endpoint,
    Method Method,
    IReadOnlyDictionary<string, string> Headers,
    IReadOnlyDictionary<string, string> Query,
    string? Body);

public sealed record JsonValidationResult(bool Valid, IReadOnlyList<string> Diagnostics);

public sealed class HttpRequestPlanner
{
    private static readonly IReadOnlySet<string> SensitiveHeaders =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Authorization",
            "Proxy-Authorization",
            "X-Api-Key",
            "Cookie",
            "Set-Cookie",
        };

    public RestRequest CreateRequest(HttpRequestPlan plan)
    {
        ArgumentNullException.ThrowIfNull(plan);
        if (plan.Endpoint.Scheme is not ("http" or "https"))
        {
            throw new ArgumentException("Only HTTP and HTTPS endpoints are supported.", nameof(plan));
        }

        RestRequest request = new(plan.Endpoint.PathAndQuery, plan.Method);
        foreach (KeyValuePair<string, string> header in plan.Headers)
        {
            request.AddOrUpdateHeader(header.Key, header.Value);
        }

        foreach (KeyValuePair<string, string> parameter in plan.Query)
        {
            request.AddQueryParameter(parameter.Key, parameter.Value);
        }

        if (!string.IsNullOrWhiteSpace(plan.Body))
        {
            request.AddStringBody(plan.Body, ContentType.Json);
        }

        return request;
    }

    public Uri BuildEndpoint(
        Uri baseUri,
        string relativePath,
        IReadOnlyDictionary<string, string> query)
    {
        ArgumentNullException.ThrowIfNull(baseUri);
        if (!baseUri.IsAbsoluteUri || baseUri.Scheme is not ("http" or "https"))
        {
            throw new ArgumentException("Base URI must be an absolute HTTP or HTTPS URI.", nameof(baseUri));
        }

        Uri endpoint = new(baseUri, relativePath.TrimStart('/'));
        IEnumerable<KeyValuePair<string, string?>> nullableQuery = query.Select(
            static item => new KeyValuePair<string, string?>(item.Key, item.Value));
        string url = QueryHelpers.AddQueryString(endpoint.ToString(), nullableQuery);
        return new Uri(url, UriKind.Absolute);
    }

    public IReadOnlyDictionary<string, string> CreateDiagnosticHeaders(
        IReadOnlyDictionary<string, string> headers)
    {
        return headers.ToDictionary(
            static item => item.Key,
            static item => SensitiveHeaders.Contains(item.Key) ? "[REDACTED]" : item.Value,
            StringComparer.OrdinalIgnoreCase);
    }

    public string ExtractTextPreview(string html, int maximumLength)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(maximumLength, 1);
        HtmlDocument document = new();
        document.LoadHtml(html ?? string.Empty);
        string decoded = System.Net.WebUtility.HtmlDecode(document.DocumentNode.InnerText);
        string normalized = Regex.Replace(decoded, @"\s+", " ").Trim();
        return normalized.Length <= maximumLength
            ? normalized
            : string.Concat(normalized.AsSpan(0, maximumLength), "\u2026");
    }

    public JsonValidationResult ValidateJson(string json, string schema)
    {
        JToken token = JToken.Parse(json);
        JSchema parsedSchema = JSchema.Parse(schema);
        bool valid = token.IsValid(parsedSchema, out IList<string> errors);
        return new JsonValidationResult(valid, errors.ToArray());
    }

    public async ValueTask<JsonValidationResult> ValidateJsonAsync(
        string json,
        string schema,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        NJsonSchema.JsonSchema parsedSchema =
            await NJsonSchema.JsonSchema.FromJsonAsync(schema, cancellationToken).ConfigureAwait(false);
        NJsonSchema.Validation.ValidationError[] errors = parsedSchema.Validate(json).ToArray();
        return new JsonValidationResult(
            errors.Length == 0,
            errors.Select(static error => error.ToString()).ToArray());
    }
}
