namespace AtlasOps.Serialization;

using System.Globalization;
using System.IO.Compression;

using CsvHelper;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using MessagePack;
using ProtoBuf;
using Scriban;
using SharpCompress.Archives;
using YamlDotNet.Serialization;

[MessagePackObject]
[ProtoContract]
public sealed class InterchangeRecord
{
    [Key(0)]
    [ProtoMember(1)]
    public string Id { get; set; } = string.Empty;

    [Key(1)]
    [ProtoMember(2)]
    public string Category { get; set; } = string.Empty;

    [Key(2)]
    [ProtoMember(3)]
    public decimal Value { get; set; }
}

public sealed class InterchangeService
{
    public Type ArchiveIntegrationType => typeof(IArchive);

    public byte[] SerializeMessagePack(IReadOnlyList<InterchangeRecord> records)
    {
        return MessagePackSerializer.Serialize(records);
    }

    public byte[] SerializeProtobuf(IReadOnlyList<InterchangeRecord> records)
    {
        using MemoryStream stream = new();
        ProtoBuf.Serializer.Serialize(stream, records);
        return stream.ToArray();
    }

    public string SerializeYaml(IReadOnlyList<InterchangeRecord> records)
    {
        SerializerBuilder builder = new();
        YamlDotNet.Serialization.ISerializer serializer = builder.Build();
        return serializer.Serialize(records);
    }

    public string SerializeCsv(IReadOnlyList<InterchangeRecord> records)
    {
        using StringWriter writer = new(CultureInfo.InvariantCulture);
        using CsvWriter csv = new(writer, CultureInfo.InvariantCulture);
        csv.WriteRecords(records);
        return writer.ToString();
    }

    public string RenderSummary(string templateText, IReadOnlyList<InterchangeRecord> records)
    {
        Template template = Template.Parse(templateText);
        return template.HasErrors
            ? string.Join(Environment.NewLine, template.Messages.Select(static message => message.Message))
            : template.Render(new { records, total = records.Sum(static record => record.Value) });
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
