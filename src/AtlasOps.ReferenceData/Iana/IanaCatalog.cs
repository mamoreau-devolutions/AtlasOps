namespace AtlasOps.ReferenceData.Iana;

using AtlasOps.ReferenceData.Infrastructure;

public sealed record IanaServiceAssignment(
    string ServiceName,
    int PortStart,
    int PortEnd,
    string Transport,
    string Description,
    string Reference);

public sealed record IanaProtocolAssignment(
    int NumberStart,
    int NumberEnd,
    string Keyword,
    string Protocol,
    string Description);

public sealed record IanaCipherSuite(
    string Value,
    string Description,
    bool DtlsCompatible,
    string Recommendation,
    string Reference);

public sealed record IanaCatalog(
    ReferencePackManifest Manifest,
    IReadOnlyList<IanaServiceAssignment> Services,
    IReadOnlyList<IanaProtocolAssignment> Protocols,
    IReadOnlyList<IanaCipherSuite> CipherSuites);

public sealed class IanaCatalogLoader
{
    public async Task<IanaCatalog> LoadAsync(
        string directory,
        CancellationToken cancellationToken = default)
    {
        ReferencePackManifest manifest = await ReferenceManifestLoader.LoadAsync(directory, cancellationToken);
        IReadOnlyList<IanaServiceAssignment> services = await this.LoadServicesAsync(directory, cancellationToken);
        IReadOnlyList<IanaProtocolAssignment> protocols = await this.LoadProtocolsAsync(directory, cancellationToken);
        IReadOnlyList<IanaCipherSuite> ciphers = await this.LoadCipherSuitesAsync(directory, cancellationToken);
        return new IanaCatalog(manifest, services, protocols, ciphers);
    }

    private async Task<IReadOnlyList<IanaServiceAssignment>> LoadServicesAsync(
        string directory,
        CancellationToken cancellationToken)
    {
        List<IanaServiceAssignment> result = [];
        Dictionary<string, int>? header = null;
        await foreach (IReadOnlyList<string> row in QuotedCsvReader.ReadAsync(
            Path.Combine(directory, "service-names-port-numbers.csv"),
            cancellationToken))
        {
            if (header is null)
            {
                header = QuotedCsvReader.CreateHeaderMap(row);
                continue;
            }

            string port = QuotedCsvReader.Get(row, header, "Port Number");
            if (!TryParseRange(port, out int start, out int end))
            {
                continue;
            }

            result.Add(new IanaServiceAssignment(
                QuotedCsvReader.Get(row, header, "Service Name"),
                start,
                end,
                QuotedCsvReader.Get(row, header, "Transport Protocol"),
                QuotedCsvReader.Get(row, header, "Description"),
                QuotedCsvReader.Get(row, header, "Reference")));
        }

        return result;
    }

    private async Task<IReadOnlyList<IanaProtocolAssignment>> LoadProtocolsAsync(
        string directory,
        CancellationToken cancellationToken)
    {
        List<IanaProtocolAssignment> result = [];
        Dictionary<string, int>? header = null;
        await foreach (IReadOnlyList<string> row in QuotedCsvReader.ReadAsync(
            Path.Combine(directory, "protocol-numbers-1.csv"),
            cancellationToken))
        {
            if (header is null)
            {
                header = QuotedCsvReader.CreateHeaderMap(row);
                continue;
            }

            string number = QuotedCsvReader.Get(row, header, "Decimal");
            if (!TryParseRange(number, out int start, out int end))
            {
                continue;
            }

            result.Add(new IanaProtocolAssignment(
                start,
                end,
                QuotedCsvReader.Get(row, header, "Keyword"),
                QuotedCsvReader.Get(row, header, "Protocol"),
                QuotedCsvReader.Get(row, header, "Description")));
        }

        return result;
    }

    private async Task<IReadOnlyList<IanaCipherSuite>> LoadCipherSuitesAsync(
        string directory,
        CancellationToken cancellationToken)
    {
        List<IanaCipherSuite> result = [];
        Dictionary<string, int>? header = null;
        await foreach (IReadOnlyList<string> row in QuotedCsvReader.ReadAsync(
            Path.Combine(directory, "tls-parameters-4.csv"),
            cancellationToken))
        {
            if (header is null)
            {
                header = QuotedCsvReader.CreateHeaderMap(row);
                continue;
            }

            string value = QuotedCsvReader.Get(row, header, "Value");
            if (string.IsNullOrWhiteSpace(value))
            {
                continue;
            }

            result.Add(new IanaCipherSuite(
                value,
                QuotedCsvReader.Get(row, header, "Description"),
                QuotedCsvReader.Get(row, header, "DTLS-OK").Equals("Y", StringComparison.OrdinalIgnoreCase),
                QuotedCsvReader.Get(row, header, "Recommended"),
                QuotedCsvReader.Get(row, header, "Reference")));
        }

        return result;
    }

    private static bool TryParseRange(string value, out int start, out int end)
    {
        string[] parts = value.Split('-', 2, StringSplitOptions.TrimEntries);
        if (parts.Length > 0 && int.TryParse(parts[0], out start))
        {
            end = parts.Length == 2 && int.TryParse(parts[1], out int parsedEnd)
                ? parsedEnd
                : start;
            return true;
        }

        start = 0;
        end = 0;
        return false;
    }
}

public sealed class IanaRegistryAnalyzer
{
    private IanaCatalog? indexedCatalog;
    private Dictionary<string, IReadOnlyList<IanaServiceAssignment>> portIndex =
        new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyList<IanaServiceAssignment> FindAssignments(
        IanaCatalog catalog,
        int port,
        string? transport = null)
    {
        this.EnsurePortIndex(catalog);
        if (!string.IsNullOrWhiteSpace(transport))
        {
            return this.portIndex.GetValueOrDefault($"{transport}:{port}") ?? [];
        }

        return this.portIndex
            .Where(item => item.Key.EndsWith($":{port}", StringComparison.Ordinal))
            .SelectMany(static item => item.Value)
            .OrderBy(static item => item.PortEnd - item.PortStart)
            .ThenBy(static item => item.ServiceName, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    public IReadOnlyList<IanaServiceAssignment> FindCollisions(IanaCatalog catalog)
    {
        return catalog.Services
            .GroupBy(
                static item => $"{item.Transport}:{item.PortStart}:{item.PortEnd}",
                StringComparer.OrdinalIgnoreCase)
            .Where(static group => group.Select(static item => item.ServiceName).Distinct().Count() > 1)
            .SelectMany(static group => group)
            .ToArray();
    }

    public bool IsCipherAllowed(IanaCipherSuite cipher, bool rejectDiscouraged)
    {
        return !cipher.Recommendation.Equals("N", StringComparison.OrdinalIgnoreCase) &&
               (!rejectDiscouraged ||
                !cipher.Recommendation.Equals("D", StringComparison.OrdinalIgnoreCase));
    }

    public string ClassifyPort(int port)
    {
        return port switch
        {
            >= 0 and <= 1023 => "Well-known",
            >= 1024 and <= 49151 => "Registered",
            >= 49152 and <= 65535 => "Dynamic or private",
            _ => "Invalid",
        };
    }

    private void EnsurePortIndex(IanaCatalog catalog)
    {
        if (ReferenceEquals(this.indexedCatalog, catalog))
        {
            return;
        }

        Dictionary<string, List<IanaServiceAssignment>> index =
            new(StringComparer.OrdinalIgnoreCase);
        foreach (IanaServiceAssignment assignment in catalog.Services)
        {
            for (int port = assignment.PortStart; port <= assignment.PortEnd; port++)
            {
                string key = $"{assignment.Transport}:{port}";
                if (!index.TryGetValue(key, out List<IanaServiceAssignment>? matches))
                {
                    matches = [];
                    index[key] = matches;
                }

                matches.Add(assignment);
            }
        }

        this.portIndex = index.ToDictionary(
            static item => item.Key,
            static item => (IReadOnlyList<IanaServiceAssignment>)item.Value
                .OrderBy(static assignment => assignment.PortEnd - assignment.PortStart)
                .ThenBy(static assignment => assignment.ServiceName, StringComparer.OrdinalIgnoreCase)
                .ToArray(),
            StringComparer.OrdinalIgnoreCase);
        this.indexedCatalog = catalog;
    }
}

public static class IanaWorkbenchBuilder
{
    public static async Task<ReferenceWorkbenchSnapshot> BuildAsync(CancellationToken cancellationToken = default)
    {
        IanaCatalog catalog = await new IanaCatalogLoader().LoadAsync(
            ReferenceDataPaths.GetPackDirectory("iana"),
            cancellationToken);
        IanaRegistryAnalyzer analyzer = new();
        List<ReferenceDataRow> rows = [];
        rows.AddRange(catalog.Services.Select(static item => new ReferenceDataRow(
            $"{item.Transport}:{item.PortStart}-{item.PortEnd}:{item.ServiceName}",
            "Service / port",
            string.IsNullOrWhiteSpace(item.ServiceName) ? "Unassigned" : item.ServiceName,
            $"{item.PortStart}-{item.PortEnd}/{item.Transport} · {item.Description}",
            $"{item.ServiceName} {item.Description} {item.Transport} {item.PortStart} {item.PortEnd} {item.Reference}")));
        rows.AddRange(catalog.Protocols.Select(static item => new ReferenceDataRow(
            $"protocol:{item.NumberStart}-{item.NumberEnd}",
            "IP protocol",
            string.IsNullOrWhiteSpace(item.Keyword) ? item.Protocol : item.Keyword,
            $"{item.NumberStart}-{item.NumberEnd} · {item.Protocol} · {item.Description}",
            $"{item.Keyword} {item.Protocol} {item.Description} {item.NumberStart} {item.NumberEnd}")));
        rows.AddRange(catalog.CipherSuites.Select(static item => new ReferenceDataRow(
            $"cipher:{item.Value}",
            "TLS cipher suite",
            item.Description,
            $"{item.Value} · Recommended: {item.Recommendation} · DTLS: {item.DtlsCompatible}",
            $"{item.Value} {item.Description} {item.Recommendation} {item.Reference}")));

        List<ReferenceValidationIssue> issues = analyzer.FindCollisions(catalog)
            .Take(500)
            .Select(static item => new ReferenceValidationIssue(
                "Information",
                "IANA-COLLISION",
                "Multiple service names share the same transport and port range.",
                $"{item.Transport}:{item.PortStart}-{item.PortEnd}"))
            .ToList();
        return new ReferenceWorkbenchSnapshot(
            "IANA network registries",
            "Ports, IP protocols, and TLS cipher policy from authoritative IANA registries.",
            catalog.Manifest,
            rows,
            issues,
            new Dictionary<string, string>
            {
                ["Service assignments"] = catalog.Services.Count.ToString("N0"),
                ["IP protocols"] = catalog.Protocols.Count.ToString("N0"),
                ["TLS cipher suites"] = catalog.CipherSuites.Count.ToString("N0"),
                ["Shared assignments"] = issues.Count.ToString("N0"),
            });
    }
}