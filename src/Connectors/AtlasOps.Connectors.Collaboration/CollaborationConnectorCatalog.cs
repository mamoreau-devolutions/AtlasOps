namespace AtlasOps.Connectors.Collaboration;

using AtlasOps.Connectors.Contracts;

using Google.Apis.Services;

using MailKit.Net.Smtp;

using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Graph;
using Microsoft.Kiota.Abstractions;

using Octokit;

public sealed record CollaborationConnectorDescriptor(
    string Id,
    string DisplayName,
    Type ClientType,
    string Protocol,
    IReadOnlyList<string> Capabilities);

public static class CollaborationConnectorCatalog
{
    public static IReadOnlyList<CollaborationConnectorDescriptor> Connectors { get; } =
    [
        new("github", "GitHub", typeof(GitHubClient), "HTTPS", ["repositories", "issues", "pull requests"]),
        new("microsoft-graph", "Microsoft Graph", typeof(GraphServiceClient), "HTTPS", ["users", "groups", "devices"]),
        new("kiota", "Kiota client", typeof(RequestInformation), "HTTPS", ["request adapters", "serialization"]),
        new("smtp", "SMTP mail", typeof(SmtpClient), "SMTP", ["mail delivery", "TLS"]),
        new("google-api", "Google APIs", typeof(BaseClientService), "HTTPS", ["discovery clients", "OAuth scopes"]),
        new("signalr", "SignalR", typeof(HubConnection), "WebSocket", ["streaming", "automatic reconnect"]),
        new("graph-core", "Microsoft Graph Core", typeof(GraphServiceClient), "HTTPS", ["middleware", "request pipeline"]),
    ];

    public static HubConnection CreateSignalRConnection(Uri endpoint)
    {
        ArgumentNullException.ThrowIfNull(endpoint);
        if (endpoint.Scheme is not ("http" or "https"))
        {
            throw new ArgumentException("SignalR endpoints must use HTTP or HTTPS.", nameof(endpoint));
        }

        return new HubConnectionBuilder()
            .WithUrl(endpoint)
            .WithAutomaticReconnect(
            [
                TimeSpan.Zero,
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(30),
            ])
            .Build();
    }

    public static IReadOnlyList<ConnectorDefinition> CreateDefinitions()
    {
        return Connectors.Select(
            static item => new ConnectorDefinition(
                item.Id,
                item.DisplayName,
                ConnectorKind.Collaboration,
                true,
                4,
                120,
                3,
                TimeSpan.FromSeconds(30),
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["protocol"] = item.Protocol,
                    ["clientType"] = item.ClientType.FullName ?? item.ClientType.Name,
                })).ToArray();
    }
}
