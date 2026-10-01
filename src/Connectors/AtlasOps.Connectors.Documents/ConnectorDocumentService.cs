namespace AtlasOps.Connectors.Documents;

using System.Formats.Cbor;
using System.IO.Compression;
using System.Net.Http.Formatting;
using System.Text;

using ICSharpCode.SharpZipLib.Zip;

using MarkdownSharp;

using OpenMcdf;

using SkiaSharp;

using Svg.Skia;

public sealed record DocumentPackageDescriptor(string Id, Type PrimaryType, string Capability);

public sealed record ConnectorDocument(string Name, string MediaType, byte[] Content);

public static class DocumentPackageCatalog
{
    public static IReadOnlyList<DocumentPackageDescriptor> Packages { get; } =
    [
        new("markdown", typeof(Markdown), "Markdown rendering"),
        new("web-api-client", typeof(JsonMediaTypeFormatter), "HTTP media formatting"),
        new("openmcdf", typeof(Storage), "Compound file inspection"),
        new("sharpziplib", typeof(ZipOutputStream), "Archive creation"),
        new("skiasharp", typeof(SKBitmap), "Bitmap rendering"),
        new("svg-skia", typeof(SKSvg), "SVG rendering"),
        new("cbor", typeof(CborWriter), "Compact manifest encoding"),
    ];
}

public sealed class ConnectorDocumentService
{
    public string RenderMarkdown(string markdown)
    {
        return new Markdown().Transform(markdown ?? string.Empty);
    }

    public byte[] CreateArchive(IEnumerable<ConnectorDocument> documents)
    {
        ArgumentNullException.ThrowIfNull(documents);
        using MemoryStream output = new();
        using (ZipArchive archive = new(output, ZipArchiveMode.Create, true))
        {
            foreach (ConnectorDocument document in documents)
            {
                ValidateName(document.Name);
                ZipArchiveEntry entry = archive.CreateEntry(document.Name, CompressionLevel.Optimal);
                using Stream stream = entry.Open();
                stream.Write(document.Content);
            }
        }

        return output.ToArray();
    }

    public IReadOnlyList<ConnectorDocument> ReadArchive(byte[] archiveBytes)
    {
        ArgumentNullException.ThrowIfNull(archiveBytes);
        using MemoryStream input = new(archiveBytes, writable: false);
        using ZipArchive archive = new(input, ZipArchiveMode.Read);
        List<ConnectorDocument> documents = new();
        foreach (ZipArchiveEntry entry in archive.Entries)
        {
            using Stream stream = entry.Open();
            using MemoryStream content = new();
            stream.CopyTo(content);
            documents.Add(new ConnectorDocument(entry.FullName, "application/octet-stream", content.ToArray()));
        }

        return documents;
    }

    public byte[] CreateManifest(IEnumerable<ConnectorDocument> documents)
    {
        ConnectorDocument[] items = documents.ToArray();
        CborWriter writer = new(CborConformanceMode.Canonical);
        writer.WriteStartArray(items.Length);
        foreach (ConnectorDocument document in items.OrderBy(static item => item.Name, StringComparer.Ordinal))
        {
            writer.WriteStartMap(3);
            writer.WriteTextString("name");
            writer.WriteTextString(document.Name);
            writer.WriteTextString("mediaType");
            writer.WriteTextString(document.MediaType);
            writer.WriteTextString("length");
            writer.WriteInt32(document.Content.Length);
            writer.WriteEndMap();
        }

        writer.WriteEndArray();
        return writer.Encode();
    }

    public IReadOnlyList<string> ReadManifestNames(byte[] manifest)
    {
        CborReader reader = new(manifest, CborConformanceMode.Canonical);
        int count = reader.ReadStartArray() ?? throw new InvalidDataException("Manifest must use a definite-length array.");
        List<string> names = new(count);
        for (int index = 0; index < count; index++)
        {
            int propertyCount = reader.ReadStartMap() ?? throw new InvalidDataException("Manifest entries must use definite-length maps.");
            string? name = null;
            for (int propertyIndex = 0; propertyIndex < propertyCount; propertyIndex++)
            {
                string property = reader.ReadTextString();
                switch (property)
                {
                    case "name":
                        name = reader.ReadTextString();
                        break;
                    case "mediaType":
                        reader.ReadTextString();
                        break;
                    case "length":
                        reader.ReadInt32();
                        break;
                    default:
                        reader.SkipValue();
                        break;
                }
            }

            reader.ReadEndMap();
            names.Add(name ?? throw new InvalidDataException("Manifest entry is missing a name."));
        }

        reader.ReadEndArray();
        return names;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name) ||
            Path.IsPathRooted(name) ||
            name.Contains("..", StringComparison.Ordinal) ||
            name.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
        {
            throw new ArgumentException("Document name must be a safe relative archive path.", nameof(name));
        }
    }
}
