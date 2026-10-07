namespace AtlasOps.Serialization;

using System.Buffers;
using System.Globalization;
using System.IO.Compression;
using System.Text;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Google.Protobuf;
using MessagePack;
using Scriban;
using Scriban.Runtime;
using SharpCompress.Archives;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

/// <summary>
/// An operational interchange row. Binary encodings use a fixed member order:
/// MessagePack writes a three-element array (<see cref="Id"/>, <see cref="Category"/>, <see cref="Value"/>),
/// and protobuf uses field numbers 1, 2, and 3, with <see cref="Value"/> encoded as a bcl.Decimal message.
/// </summary>
public sealed class InterchangeRecord
{
    public string Id { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public decimal Value { get; set; }
}

/// <summary>
/// Interchange encoders. Every format is written through explicit, source-generated, or
/// event-level APIs rather than reflection-based object mapping, so the service is trimming
/// and NativeAOT safe.
/// </summary>
public sealed class InterchangeService
{
    public Type ArchiveIntegrationType => typeof(IArchive);

    public byte[] SerializeMessagePack(IReadOnlyList<InterchangeRecord> records)
    {
        ArrayBufferWriter<byte> buffer = new();
        MessagePackWriter writer = new(buffer);
        writer.WriteArrayHeader(records.Count);
        foreach (InterchangeRecord record in records)
        {
            if (record is null)
            {
                writer.WriteNil();
                continue;
            }

            writer.WriteArrayHeader(3);
            writer.Write(record.Id);
            writer.Write(record.Category);
            writer.Write(record.Value.ToString(CultureInfo.InvariantCulture));
        }

        writer.Flush();
        return buffer.WrittenSpan.ToArray();
    }

    public byte[] SerializeProtobuf(IReadOnlyList<InterchangeRecord> records)
    {
        using MemoryStream stream = new();
        using (CodedOutputStream output = new(stream, leaveOpen: true))
        {
            foreach (InterchangeRecord record in records)
            {
                output.WriteTag(1, WireFormat.WireType.LengthDelimited);
                output.WriteBytes(ByteString.CopyFrom(EncodeProtobufRecord(record)));
            }
        }

        return stream.ToArray();
    }

    public string SerializeYaml(IReadOnlyList<InterchangeRecord> records)
    {
        using StringWriter writer = new(CultureInfo.InvariantCulture);
        Emitter emitter = new(writer);
        emitter.Emit(new StreamStart());
        emitter.Emit(new DocumentStart());
        emitter.Emit(new SequenceStart(null, null, false, SequenceStyle.Block));
        foreach (InterchangeRecord record in records)
        {
            emitter.Emit(new MappingStart(null, null, false, MappingStyle.Block));
            EmitYamlProperty(emitter, nameof(InterchangeRecord.Id), record.Id);
            EmitYamlProperty(emitter, nameof(InterchangeRecord.Category), record.Category);
            EmitYamlProperty(emitter, nameof(InterchangeRecord.Value), record.Value.ToString(CultureInfo.InvariantCulture));
            emitter.Emit(new MappingEnd());
        }

        emitter.Emit(new SequenceEnd());
        emitter.Emit(new DocumentEnd(true));
        emitter.Emit(new StreamEnd());
        return writer.ToString();
    }

    public string SerializeCsv(IReadOnlyList<InterchangeRecord> records)
    {
        StringBuilder csv = new();
        AppendCsvRow(csv, nameof(InterchangeRecord.Id), nameof(InterchangeRecord.Category), nameof(InterchangeRecord.Value));
        foreach (InterchangeRecord record in records)
        {
            AppendCsvRow(csv, record.Id, record.Category, record.Value.ToString(CultureInfo.InvariantCulture));
        }

        return csv.ToString();
    }

    public string RenderSummary(string templateText, IReadOnlyList<InterchangeRecord> records)
    {
        Template template = Template.Parse(templateText);
        if (template.HasErrors)
        {
            return string.Join(Environment.NewLine, template.Messages.Select(static message => message.Message));
        }

        ScriptArray recordItems = new(records.Count);
        foreach (InterchangeRecord record in records)
        {
            recordItems.Add(new ScriptObject
            {
                ["id"] = record.Id,
                ["category"] = record.Category,
                ["value"] = record.Value,
            });
        }

        ScriptObject model = new()
        {
            ["records"] = recordItems,
            ["total"] = records.Sum(static record => record.Value),
        };
        TemplateContext context = new();
        context.PushGlobal(model);
        return template.Render(context);
    }

    public byte[] CreateWorkbook(IReadOnlyList<InterchangeRecord> records)
    {
        using MemoryStream stream = new();
        using (SpreadsheetDocument document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook, true))
        {
            WorkbookPart workbookPart = document.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();
            WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            SheetData data = new();
            data.Append(CreateRow("Id", "Category", "Value"));
            foreach (InterchangeRecord record in records)
            {
                data.Append(CreateRow(record.Id, record.Category, record.Value.ToString(CultureInfo.InvariantCulture)));
            }

            worksheetPart.Worksheet = new Worksheet(data);
            Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
            sheets.Append(new Sheet
            {
                Id = workbookPart.GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = "AtlasOps",
            });
            workbookPart.Workbook.Save();
        }

        return stream.ToArray();
    }

    public byte[] CreateArchive(string entryName, byte[] content)
    {
        using MemoryStream stream = new();
        using (System.IO.Compression.ZipArchive archive = new(stream, ZipArchiveMode.Create, true))
        {
            ZipArchiveEntry entry = archive.CreateEntry(entryName, CompressionLevel.Optimal);
            using Stream entryStream = entry.Open();
            entryStream.Write(content);
        }

        return stream.ToArray();
    }

    /// <summary>
    /// Writes an RFC 4180 row: fields containing the delimiter, quotes, line breaks, or
    /// leading/trailing spaces are quoted with embedded quotes doubled; rows end with CRLF.
    /// </summary>
    private static void AppendCsvRow(StringBuilder builder, params string[] fields)
    {
        for (int index = 0; index < fields.Length; index++)
        {
            if (index > 0)
            {
                builder.Append(',');
            }

            string field = fields[index] ?? string.Empty;
            bool quote = field.Length > 0 &&
                (field.AsSpan().IndexOfAny(",\"\r\n") >= 0 || field[0] == ' ' || field[^1] == ' ');
            if (quote)
            {
                builder.Append('"').Append(field.Replace("\"", "\"\"", StringComparison.Ordinal)).Append('"');
            }
            else
            {
                builder.Append(field);
            }
        }

        builder.Append("\r\n");
    }

    /// <summary>
    /// Encodes one record as a protobuf message using the implicit-default conventions of the
    /// original protobuf-net contract: null strings and a zero value are omitted.
    /// </summary>
    private static byte[] EncodeProtobufRecord(InterchangeRecord record)
    {
        using MemoryStream stream = new();
        using (CodedOutputStream output = new(stream, leaveOpen: true))
        {
            if (record.Id is not null)
            {
                output.WriteTag(1, WireFormat.WireType.LengthDelimited);
                output.WriteString(record.Id);
            }

            if (record.Category is not null)
            {
                output.WriteTag(2, WireFormat.WireType.LengthDelimited);
                output.WriteString(record.Category);
            }

            if (record.Value != 0m)
            {
                output.WriteTag(3, WireFormat.WireType.LengthDelimited);
                output.WriteBytes(ByteString.CopyFrom(EncodeBclDecimal(record.Value)));
            }
        }

        return stream.ToArray();
    }

    /// <summary>
    /// Encodes a decimal as the bcl.Decimal message (lo = 1, hi = 2, signScale = 3) used by protobuf-net.
    /// </summary>
    private static byte[] EncodeBclDecimal(decimal value)
    {
        Span<int> bits = stackalloc int[4];
        decimal.GetBits(value, bits);
        ulong low = ((ulong)(uint)bits[1] << 32) | (uint)bits[0];
        uint high = (uint)bits[2];
        uint signScale = (uint)(((bits[3] >> 15) & 0x01FE) | ((bits[3] >> 31) & 0x0001));
        using MemoryStream stream = new();
        using (CodedOutputStream output = new(stream, leaveOpen: true))
        {
            if (low != 0)
            {
                output.WriteTag(1, WireFormat.WireType.Varint);
                output.WriteUInt64(low);
            }

            if (high != 0)
            {
                output.WriteTag(2, WireFormat.WireType.Varint);
                output.WriteUInt32(high);
            }

            if (signScale != 0)
            {
                output.WriteTag(3, WireFormat.WireType.Varint);
                output.WriteUInt32(signScale);
            }
        }

        return stream.ToArray();
    }

    private static void EmitYamlProperty(IEmitter emitter, string name, string? value)
    {
        emitter.Emit(new Scalar(name));
        emitter.Emit(new Scalar(value ?? string.Empty));
    }

    private static Row CreateRow(params string[] values)
    {
        Row row = new();
        foreach (string value in values)
        {
            row.Append(new Cell
            {
                DataType = CellValues.String,
                CellValue = new CellValue(value),
            });
        }

        return row;
    }
}
